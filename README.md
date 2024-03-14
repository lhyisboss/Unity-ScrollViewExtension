# Unity-ScrollVeiwExtension
A simple extension based on VerticalOrHorizontalLayoutGroup.

## 使用シチュエーション
#### ①データ数が多すぎ、表示際の処理が重すぎる場合
#### ②実行中にアイテムのサイズを変更したい場合
#### ③実行中にアイテム追加or削除したい場合

## 使用例
![Alt text for the GIF](/Img/preload.gif)  

![Alt text for the GIF](/Img/removeitem.gif)

## 説明
verticalやhorizontal layout groupを経由で生成数を最適化の手段の一つです。

現在の特徴：  
①最小生成数でスクロールの内容を表示していきます。  
②途中でサイズ変更は可能です。  
③indexを指定して表示することが可能です  
④barの位置を指定して表示することが可能です。  
⑤動的異なるサイズのアイテムを追加することが可能です。  
⑥動的にアイテム削除することが可能です。  
⑦spacingやpaddingはある程度使えます。

Ps:持続機能追加&リファクタリング中です。一応Clean Architectureを心掛けてコードを書いていますが、  
二兎を追う者は一兎をも得ずということで完全に原則に沿って書いたわけではないです、ご了承ください。
