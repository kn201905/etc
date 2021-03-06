# 前置き

　まずは C# の文法を覚えようと思うのだろうけど、C# はオブジェクト指向プログラミング（＝OOP）と相性がよく理解もしやすいため、まずは動くものを作ってみて感覚をつかむのが良いと思う。

　以下に示した流れでコードを書いていくと、細かい文法が分からない状態でコードができあがっていくから少々気持ち悪く感じると思う。
だけど、このページにある内容は１、２時間もあれば試せると思うので、その後に文法について調べてみてほしい。

　個人的には、文法はコードを開発しながら理解した方がいいと思っている。最近、クロージャや非同期機構が分からない、と言っている人を多く見かけるけど、そんなの文法書を読んでも理解は難しいと思う。とにかく動かしてみて、その動きから理解するのが間違いなく簡単。

# C# のインストール

　ネットで C#、インストール、と検索してもらえばよいと思う。Visual Studio をインストールするときに、「.NET デスクトップ開発」にチェックマークを付けてインストールするだけでＯＫ。Visual Studio は、後で必要なものが出てきたら、いつでも自由に追加インストールすることができるため、気楽にどうぞ。

　以下の記事が簡潔で読みやすいと思う。

https://qiita.com/grinpeaceman/items/b5a6082f94c9e4891613

# まずは動くものを作る

　ファイル -> 新規作成 -> プロジェクト で、C# の「Windows フォームアプリケーション（.NET Framework）」を作成。

　このプロジェクトは試しに作るだけなので、名前などは適当でＯＫ。分からなければ、ＯＫボタンをどんどん押していってもらえばよい（１、２時間後には消すプロジェクトになるだろうし、、）。

# とりあえず実行

　プロジェクトが開いたら、とりあえず「F5」キーで実行。（メニューを利用する場合は、デバッグ -> デバッグの開始）

　ウィンドウが１枚開けばＯＫ。「✕」ボタンでウィンドウを閉じて終了。

# 動作したファイルの確認

　できあがるファイルで重要なのは下の３つだけ。namespace のところに表示されるものは、プロジェクトを作成する時に付けた名前になっていると思う。

(ア) Program.cs（ソリューションエクスプローラで、Program.cs をダブルクリック）
```cs
namespace TestProgram
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}
```

(イ) Form1.cs（ソリューションエクスプローラで Form1.cs を右クリックして、「コードの表示」をクリック）
```cs
namespace TestProgram
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}
	}
}
```

(ウ) Form1.Designers.cs（ソリューションエクスプローラで、Forms1.cs の左側の ▶ マークをクリックすると、Form1.Designers.cs が表示されるので、それをダブルクリック）
```cs
namespace TestProgram
{
	partial class Form1
	{
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Text = "Form1";
		}

		#endregion
	}
}

```

# namespace について

　namespace は、ファイルが分かれてても同じプログラムのコードであることを示すために利用するもの。


# 実行時の動作の流れ

* C# は、プログラム中にある `static void Main()` から実行が開始される、という仕様になっている。だから、(ア) の `static void Main()` からプログラムが開始される。

* 以下の２つは、描画関連の内部スイッチの変更。時間に余裕があるときに、ネットで調べると良いと思う。詳細を知ったとしても、特に変更するものではないものだと思う。
```cs
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
```

* `Application.Run(new Form1());` について

   * `Form1` を生成（＝new）して、生成したものを `Application.Run()` に渡している。`Form1` はウィンドウを表していて、`Application.Run` はアプリの実行を行う命令であるため、この命令によりウィンドウが表示され、アプリの実行が開始される。

   * `Form1` が生成されるとき、(イ) の `public Form1()` が実行される。（この仕組については後で解説する。現段階では「何かを生成するときには、それに付随して実行されるものがある」という理解でＯＫ。）
   
   * (イ) の `Form1() { ... }` の中に、`InitializeComponent()` と書いてあるので、`InitializeComponent()` の実行に移る。`InitializeComponent()` の中身に関しては、後で分かるようになればよい。
<BR>

> **(イ) の `public partial class Form1 : Form { ... }` について**
>
>　public、partial、: Form については後で分かる。ここでは「class」について理解しておく。
>
>　`class A { ... }` とすると、`...` の部分が「A を構成する部品」という意味になるよ。「class」は、１つのまとまったものを表す。学校での「クラス」と同じ概念。
>
>　以下のように書くと、`Form1` というクラスの中に、`Form1()` という関数があるという意味になる。
> ```cs
> class Form1
> {
>	Form1()
>	{
>	}
> }
> ```

<BR>

> 「関数」という言葉について  
> 　数学での関数と同じ感覚。何かを実行して（計算して）、できたものを返す、という感覚でＯＫ。  
> `Form1()` という関数は、ウィンドウを作成して、できたウィンドウを返す、という動作をする。

<BR>

# テキストボックスを付けてみる

　まずは(イ) のファイルの、`Form1()` にコードを追加する。何らかの部品を作る場合は、次のようにする。

* `「部品の class名」 「部品に付ける名前」;` で、部品の名前を決定する。

   * (例) `Size box_size;` ▶ box_size は、`Size` というクラスの部品を表す名前。

* `「部品の名前」＝ new「class名」;` で、実際に部品を生成する。

   * (例) `box_size = new Size(100, 100);` で、横 100 pixel、縦 100 pixel の大きさの `Size` オブジェクト（＝もの）を作り、それを `box_size` に割り当てる。「横 100 pixel、縦 100 pixel」という情報を追加するために、() の中に情報を追加している。

　とりあえず、以下のようにコードを書き足して、F5 キーで実行させてみて欲しい。

```cs
namespace TestProgram
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			Size box_size;　　　　　　　　　　// 名前を付ける
			box_size = new Size(100, 100);　// new で生成する

			TextBox text_box;　　　　　　// 名前を付ける
			text_box = new TextBox();　　// new で生成する
			text_box.Size = box_size;　　
			text_box.Multiline = true;

			this.Controls.Add(text_box);   // 作ったテキストボックス text_box を、ウィンドウに追加する
		}
	}
}
```

* `TextBox` は、`Size` や `Multiline` などのプロパティ（＝属性値）を持つので、生成した後にそれらのプロパティを設定している。

   * `text_box.Multiline = true;` としておかないと、１行しか表示できないテキストボックスとなってしまう。

* 「名前を付ける」のと、「new で生成する」のを２行に分けて書くのが面倒くさい場合は、まとめることができる。以下の (a) と (b) は同じ意味になる。

(a)
```cs
TextBox text_box;
text_box = new TextBox();
```
(b)
```cs
TextBox text_box = new TextBox();
```

* `this.Controls.Add(text_box);` について

   * `this` は、`class Form1 { ... }` の中にあるので、`this` は `Form1` (F5 で実行すると表示されるウィンドウ) を指すことになる。

   * `this.Controls.Add(...)` は、`Form1` の `Controls`（＝テキストボックスやボタンなどのコントロールの集合）に、`...` を `Add` せよ、という意味。

   * `this.Controls.Add(text_box);` ＝ 「`Form1` の `Controls` に、`text_box` を `Add` する」

* この状態で F5 で実行すると、テキストボックスが張り付いたウィンドウが表示される。
　

# テキストボックスの位置を変えてみる

　TextBox の位置を変える場合、Location を変更する。以下のようなコードを追加して実験。

　C# では、名前を決めて、new で生成して、それを渡したり操作したりする、という感覚が必要。位置を指定するためだけに、 **「位置という情報を作成する」** 必要がある。  
  さっきのサイズ設定と同じ感覚。

```cs
Point box_pos;　　　　　　　　　// 名前を決める
box_pos = new Point(50, 50);　// new で生成する

text_box.Location = box_pos;　// 作った位置（Point）を渡す
```
または、
```cs
text_box.Location = new Point(50, 50);  // new で生成したものを、直接渡すこともできる
```

# ボタンを追加してみる

```cs
namespace TestProgram
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			TextBox text_box = new TextBox();
			text_box.Size = new Size(100, 100);
			text_box.Multiline = true;
			text_box.Location = new Point(50, 50);

			this.Controls.Add(text_box);


			Button btn = new Button();  // 名前を決める
			btn.Text = "ボタン";        // new で生成する

			this.Controls.Add(btn);    // 作ったボタン btn を、ウィンドウに追加する
		}
	}
}
```

# ボタンを押したら、ウィンドウが閉じるようにしてみる

* ウィンドウを閉じるときは、Form に Close() を命令すればよい。
* Button クラスで作成されたオブジェクト（＝部品）は、ボタンが押されると Click に設定されたメソッド（＝関数）を呼び出す。

```cs
namespace TestProgram
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			TextBox text_box;
			text_box = new TextBox();
			text_box.Size = new Size(100, 100);
			text_box.Multiline = true;
			text_box.Location = new Point(50, 50);

			this.Controls.Add(text_box);


			Button btn = new Button();
			btn.Text = "ボタン";
			btn.Click += OnClick_Button;   // btn が押されたら OnClick_Button を呼び出すようにする

			this.Controls.Add(btn);
		}

		// object sender, EventArgs e の意味は後で。
		// 理解できるようになるのは、かなり後になるかも、、
		void OnClick_Button(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
```

# ボタンを押したら、テキストボックスにメッセージが表示されるようにしてみる

* テキストボックスに、メッセージを追加表示させたい場合は `AppendText()` という命令を使う。
* 考え方としては以下のようになるけれど、以下のままではエラーとなる。
```cs
namespace TestProgram
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			TextBox text_box;
			text_box = new TextBox();
			text_box.Size = new Size(100, 100);
			text_box.Multiline = true;
			text_box.Location = new Point(50, 50);

			this.Controls.Add(text_box);


			Button btn = new Button();
			btn.Text = "ボタン";
			btn.Click += OnClick_Button;

			this.Controls.Add(btn);
		}

		void OnClick_Button(object sender, EventArgs e)
		{
			text_box.AppendText("Hello.\r\n");  // \r\n は改行の意味
		}
	}
}
```

上記のコードでは、`text_box.AppendText("Hello.\r\n");` を実行しようとするときに、「`text_box` が指しているものを見つけられない」というエラーが発生する。


* エラーとなる理由

　変数などに付けた名前の有効範囲は { } の内側に制限されるため。

* エラーの解消法

　名前の有効範囲を広めるために、{ } の外側で名前を宣言する。

```cs
namespace TestProgram
{
	public partial class Form1 : Form
	{
		// ここで、text_box という名前を作っておけば、
		// この名前は Form1() からも OnClick_Button() からも共通して見ることができる。
		TextBox text_box;

		public Form1()
		{
			InitializeComponent();

			text_box = new TextBox();
			text_box.Size = new Size(100, 100);
			text_box.Multiline = true;
			text_box.Location = new Point(50, 50);

			this.Controls.Add(text_box);


			Button btn = new Button();
			btn.Text = "ボタン";
			btn.Click += OnClick_Button;

			this.Controls.Add(btn);
		}

		void OnClick_Button(object sender, EventArgs e)
		{
			text_box.AppendText("Hello.\r\n");
		}
	}
}
```
　名前を作ると同時に new をすることもできるから、以下のように書いてもよい。（`new TextBox();` の場所が変わっただけ）

```cs
namespace TestProgram
{
	public partial class Form1 : Form
	{
		TextBox text_box = new TextBox();

		public Form1()
		{
			InitializeComponent();

			text_box.Size = new Size(100, 100);
			text_box.Multiline = true;
			text_box.Location = new Point(50, 50);

			this.Controls.Add(text_box);


			Button btn = new Button();
			btn.Text = "ボタン";
			btn.Click += OnClick_Button;

			this.Controls.Add(btn);
		}

		void OnClick_Button(object sender, EventArgs e)
		{
			text_box.AppendText("Hello.\r\n");
		}
	}
}
```

---
# 02.md に続く
