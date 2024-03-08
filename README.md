# Unity-ScrollVeiwExtension
A simple extension based on VerticalOrHorizontalLayoutGroup.

## 使用シチュエーション
#### ①データ数が多すぎ、表示際の処理が重すぎる場合
#### ②実行中に動的でアイテムのサイズを変更したい場合
#### ③実行中に動的でアイテム追加したい場合

## Verticalとhorizontal方向のプレビュー
![Alt text for the GIF](/Img/vertical.gif)  

![Alt text for the GIF](/Img/horizontal.gif)  

![Alt text for the GIF](/Img/preload.gif)  


## 説明
verticalやhorizontal layout groupを経由で生成数を最適化の手段の一つです。

現在の特徴：  
①最小生成数でスクロールの内容を表示していきます。  
②途中でサイズ変更は可能です。  
③indexを指定して表示することが可能です  
④barの位置を指定して表示することが可能です。  
⑤動的異なるサイズのアイテムを追加することが可能です。
⑥spacingやpaddingは普通に使えます。

一応Clean Architectureを心掛けてコードを書いてました。  
