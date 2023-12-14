# Unity-ScrollVeiwExtension
A simple extension based on VerticalOrHorizontalLayoutGroup and GridLayoutGroup.

## Verticalとhorizontal方向のDemo
![Alt text for the GIF](/Img/vertical.gif)  

![Alt text for the GIF](/Img/horizontal.gif)  

demoは501個のデータを表示する例です。  
index,bar位置を指定して表示することもできます。  
途中サイズ変更も可能です。

## 説明
verticalやhorizontal layout groupを経由で生成数を最適化の手段の一つです。

現在何ができる：  
①最小生成数でスクロールの内容を表示していきます。  
②途中でサイズ変更は可能です。  
③indexを指定して表示することが可能です  
④barの位置を指定して表示することが可能です。  

Clean Architectureを原則して構成されました。  

タイプ依頼:  
<img src="./Img/type dependency digram.png" width="80%" height="80%">  

※：  
自分は最近あったケースは以上の機能で大体足りますが、もし他に何か入って欲しい機能があったら、  
全然追加しますので、是非知らせください。もしくはforkしてPR出していただいても大丈夫です。  
GridLayout経由の拡張も今作成中で、次のバージョンでまとめてアップします。
