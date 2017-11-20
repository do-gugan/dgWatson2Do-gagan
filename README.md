# dgWatson2Do-gagan
Watson Speech To Textサービスの返すJSONを動画眼で読み込めるタブ区切りテキストファイルに変換します。
動画眼はタイムスタンプと発言内容をペアにしたタブ区切りファイルを読み込み、発言内容テキストを選択すると動画の当該部分を頭出し再生する、ユーザテスト分析などに使用するツールです。

Watsonの返すJSONファイルは、解析時のオプション指定に
- timestamps=true
- speaker_labels=true
- word_confidence=false

をつけたものをしています。返されたファイルにはtranscriptフィールドにセンテンスが書き込まれていますが、途中で話者が切り替わっていてもひとくくりにされてしまいます。ユーザテストの発話記録としてはこれでは不便なので、transcriptフィールドは捨てて、同じalternativesセットにあるtimestampsオブジェクトを利用し、その開始タイムスタンプを、speaker_labels=trueオプションで付加されるspeaker_labels配列を参照して話者を調べることで、できるだけ正確に話者毎の行替えするようにしています。

#使用方法
コマンドプロンプトで、解析したファイル名を起動オプションにつけて実行します。

c:¥ > dgWatson2Do-gagan json_from_watson.txt

同一フォルダに拡張子.txtを付加したファイル名で保存されます（例の場合、json_from_watson.txt.txt）。

#動作要件
- Windows
- .NET Framework 4.6.1

開発自体はVisual Studio for Macで行っているので、Macで実行可能なビルドも可能ははずですがまだ詳しく調べていません。

# クレジット
JSONデータのパースに、MicrosoftのDynamicJsonライブラリを使用しています。
