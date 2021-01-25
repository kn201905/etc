
static class FLogs
{
	public static File_Log_SingleThread FLog = null;
}

///////////////////////////////////////////////////////////////////////////////////////

class File_Log_SingleThread : IDisposable
{
	const int EN_thld_mem_buf = 1024;  // メモリストリームの閾値
	const int EN_size_mem_buf = 2 * 1024;  // メモリストリームサイズの初期値
	const int EN_size_file_buf = 8 * 1024;  // ファイルストリームサイズの初期値

	MemoryStream m_ms = new MemoryStream(EN_size_mem_buf);
	FileStream m_fs = null;

	string m_fname;
	string m_str_path_direcotory;  // コンストラクタで設定する
	string m_str_format_datetime;

	int m_bytes_split_log = 0;

	const int EN_bytes_buf_for_wrtg_ms = 1024;
	byte[] m_ary_buf_for_wrtg_ms = new byte[EN_bytes_buf_for_wrtg_ms];  // メモリストリームに書き込む時に利用されるバッファ

	// ------------------------------------------------------------------------------------
	// format_datetime は、「yy-MM-dd-HH-mm-ss」を参考に
	// 拡張子は自動的に「.txt」となる
	public File_Log_SingleThread(string fname, string format_datetime = null, string directory_path = null)
	{
		m_fname = fname;

		if (directory_path == null)
		{
			// m_str_path_direcotory はカレントディレクリを指すことになる
			m_str_path_direcotory = "./";
		}
		else
		{
			m_str_path_direcotory = directory_path;
		}

		m_str_format_datetime = format_datetime;

		// まず、ファイル名を決定する
		string fullpath;
		if (format_datetime == null)
		{
			fullpath = m_str_path_direcotory + fname + ".txt";
		}
		else
		{
			fullpath = m_str_path_direcotory + fname + DateTime.Now.ToString(format_datetime) + ".txt";
		}

		m_fs = new FileStream(
			fullpath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, EN_size_file_buf);

		// ファイルは追記していくことにする
		m_fs.Position = m_fs.Length;
	}

	// ------------------------------------------------------------------------------------
	// メモリストリームの内容が閾値を超えていたらファイルストリームに書き出す
	void CheckWrtOut()
	{
		if (m_bytes_split_log > 0)
		{
			if (m_fs.Length > m_bytes_split_log)
			{
				m_ms.WriteTo(m_fs);
				m_ms.Position = 0;
				m_ms.SetLength(0);

				m_fs.Flush();
				m_fs.Dispose();

				// ----------------------------------------------------
				// 次のファイルの準備
				string fullpath = m_str_path_direcotory + m_fname + DateTime.Now.ToString(m_str_format_datetime) + ".txt";
				m_fs = new FileStream(
					fullpath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, EN_size_file_buf);

				// open したファイルが同一ファイルとなる可能性もあるため
				m_fs.Position = m_fs.Length;
				return;
			}
		}

		if (m_ms.Position > EN_thld_mem_buf)
		{
			m_ms.WriteTo(m_fs);
			m_ms.Position = 0;
			m_ms.SetLength(0);

			m_fs.Flush();  /// TODO: filestream を Flush させる頻度は、今後調整すること
		}
	}

	// ------------------------------------------------------------------------------------
	public void Dispose()
	{
		if (m_ms.Position > 0)
		{
			m_ms.WriteTo(m_fs);
		}
		m_fs.Flush();

		m_fs.Dispose();
		m_ms.Dispose();
	}

	// ------------------------------------------------------------------------------------
	public void Set_bytes_split_log(int bytes)
	{
		// 小さすぎる閾値は無視する
		if (bytes < 1024) { return; }

		m_bytes_split_log = bytes;

		if (m_str_format_datetime == null)
		{
			m_str_format_datetime = "MM-dd-HH-mm-ss";
		}
	}

	// ------------------------------------------------------------------------------------
	public void Split_Log()
	{
		m_ms.WriteTo(m_fs);
		m_ms.Position = 0;
		m_ms.SetLength(0);

		m_fs.Flush();
		m_fs.Dispose();

		// ----------------------------------------------------
		// 次のファイルの準備
		string fullpath = m_str_path_direcotory + m_fname + DateTime.Now.ToString(m_str_format_datetime) + ".txt";
		m_fs = new FileStream(
			fullpath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, EN_size_file_buf);

		// open したファイルが同一ファイルとなる可能性もあるため
		m_fs.Position = m_fs.Length;
	}

	// ------------------------------------------------------------------------------------
	// m_ary_buf_for_wrtg_ms に、"  [HH:mm:ss]  " を書き込む。「#」等は、この関数から戻った後に書き込むこと
	unsafe void Wrt_Time_to_ary_buf(byte* ptr)
	{
		DateTime dtime = DateTime.Now;
		int hour = dtime.Hour;
		int minu = dtime.Minute;
		int sec = dtime.Second;

		// "  [HH:mm" の書き込み
		*(ulong*)ptr = 0x3030_3a30_305b_2020 + ((ulong)(minu % 10) << 56) + ((ulong)(minu / 10) << 48)
														+ ((ulong)(hour % 10) << 32) + ((ulong)(hour / 10) << 24);
		// ":ss]    "
		*(ulong*)(ptr + 8) = 0x2020_2020_5d30_303a + ((ulong)(sec % 10) << 16) + ((ulong)(sec / 10) << 8);
	}

	// ------------------------------------------------------------------------------------
	public void Write(string str)
	{
		byte[] ary_buf = Encoding.UTF8.GetBytes(str);
		m_ms.Write(ary_buf, 0, ary_buf.Length);
		CheckWrtOut();
	}

	public unsafe void Wrt_wTime(string str)
	{
		fixed (byte* pTop_dst = m_ary_buf_for_wrtg_ms)
		fixed (Char* pTop_src = str)
		{
			Wrt_Time_to_ary_buf(pTop_dst);
			int bytes_wrt = Encoding.UTF8.GetBytes(pTop_src, str.Length, pTop_dst + 14, EN_bytes_buf_for_wrtg_ms - 14);
			m_ms.Write(m_ary_buf_for_wrtg_ms, 0, bytes_wrt + 14);
		}
		CheckWrtOut();
	}

	public unsafe void Wrt_SIG_wTime(string str)
	{
		fixed (byte* pTop_dst = m_ary_buf_for_wrtg_ms)
		fixed (Char* pTop_src = str)
		{
			Wrt_Time_to_ary_buf(pTop_dst);
			*pTop_dst = 0x23;  // "#"
			int bytes_wrt = Encoding.UTF8.GetBytes(pTop_src, str.Length, pTop_dst + 14, EN_bytes_buf_for_wrtg_ms - 14);
			m_ms.Write(m_ary_buf_for_wrtg_ms, 0, bytes_wrt + 14);
		}
		CheckWrtOut();
	}

	public unsafe void Wrt_ERR_wTime(string str)
	{
		fixed (byte* pTop_dst = m_ary_buf_for_wrtg_ms)
		fixed (Char* pTop_src = str)
		{
			Wrt_Time_to_ary_buf(pTop_dst);
			*(ushort*)pTop_dst = 0x2323;  // "##"
			int bytes_wrt = Encoding.UTF8.GetBytes(pTop_src, str.Length, pTop_dst + 14, EN_bytes_buf_for_wrtg_ms - 14);
			m_ms.Write(m_ary_buf_for_wrtg_ms, 0, bytes_wrt + 14);
		}
		CheckWrtOut();
	}

	// ------------------------------------------------------------------------------------
	public unsafe void Wrt_ASCII_wTime(string str)
	{
		fixed (byte* pTop_dst = m_ary_buf_for_wrtg_ms)
		fixed (Char* pTop_src = str)
		{
			Wrt_Time_to_ary_buf(pTop_dst);

			byte* pdst = pTop_dst + 14;
			ushort* psrc = (ushort*)pTop_src;

			int len = str.Length;
			if (len > EN_bytes_buf_for_wrtg_ms - 14)
			{ len = EN_bytes_buf_for_wrtg_ms - 14; }

			for (; len > 0; --len)
			{
				*pdst++ = (byte)*psrc++;
			}

			m_ms.Write(m_ary_buf_for_wrtg_ms, 0, (int)(pdst - pTop_dst));
		}
		CheckWrtOut();
	}

	// ------------------------------------------------------------------------------------
	public unsafe void Wrt_wTime_1Line_from_tcp_buf(byte* psrc, byte* pTmnt_src)
	{
		fixed (byte* pTop_dst = m_ary_buf_for_wrtg_ms)
		{
			Wrt_Time_to_ary_buf(pTop_dst);

			byte* pdst = pTop_dst + 14;
			byte* pTmnt_dst = pTop_dst + EN_bytes_buf_for_wrtg_ms;

			while ((*pdst++ = *psrc++) != '\n')
			{
				if (psrc == pTmnt_src || pdst == pTmnt_dst) { break; }
			}

			m_ms.Write(m_ary_buf_for_wrtg_ms, 0, (int)(pdst - pTop_dst));
		}
		CheckWrtOut();
	}
} // class File_Log_SingleThread
