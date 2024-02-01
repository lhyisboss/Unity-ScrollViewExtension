# Unity-ScrollVeiwExtension
A simple extension based on VerticalOrHorizontalLayoutGroup and GridLayoutGroup.

## Verticalとhorizontal方向のプレビュー
![Alt text for the GIF](/Img/vertical.gif)  

![Alt text for the GIF](/Img/horizontal.gif)  

![Alt text for the GIF](/Img/additem.gif)  

プレビューは501個のデータを表示する例です。  
index,bar位置を指定して表示することもできます。  
途中サイズ変更も可能です。  
動的異なるサイズの新しいアイテムを追加することも可能です。

## 説明
verticalやhorizontal layout groupを経由で生成数を最適化の手段の一つです。

現在の特徴：  
①最小生成数でスクロールの内容を表示していきます。  
②途中でサイズ変更は可能です。  
③indexを指定して表示することが可能です  
④barの位置を指定して表示することが可能です。  
⑤動的異なるサイズのアイテムを追加することが可能です。  

Clean Architectureを原則して構成されました。  
