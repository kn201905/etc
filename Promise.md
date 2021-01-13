```
new Promise(f => {  // 敢えて f と書いている（普通は resolve と書くところ）
	console.log('--- A ---');
	WaitForClient(f);
	console.log('--- B ---');
})
.then((msg) => {
	console.log('--- クライアント接続 ---');
	console.log(msg);
});

console.log('--- C ---');

// --------------

function WaitForClient(f_cb) {
	setTimeout(() => {
		f_cb('connected');
	}, 2000);
}
```

上のプロミスを、擬似的に書くと以下のようになる。<br>
（話を簡単にするため、rejecter は書いてない）

```
function Promise(F) {
  function resolve() {  // <- これが resolver。この場合、then に渡された関数を実行するのが resolver の仕事
     ...
  }
  
  F(resolve);
}
```

rejecter も付け加えると

```
function Promise(F) {
  function resolve() {  // <- resolver。この場合、then に渡された関数を実行するのが resolver の仕事
     ...
  }
  function reject() {
     ...
  }
  
  F(resolve, reject);
}
```

---

## サンプル
```
new Promise(f => {  // 敢えて f と書いている（普通は resolve と書くところ）
	console.log('--- A ---');
	setTimeout(() => {
		f('resolved');
	}, 2000);
	console.log('--- B ---');
})
.then((msg) => {
	console.log('--- C ---');
	console.log(msg);
});
```

```
function Promise(F) {
  function resolve() {  // <- これが resolver。この場合、then に渡された関数を実行するのが resolver の仕事
     ...
  }
  
  F(resolve);
}
```
