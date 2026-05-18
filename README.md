\# 3Dグリッド迷宮探索テンプレート



Unity初心者向けの、固定マップ式3Dダンジョン探索テンプレートです。



プレイヤーは一人称視点で3D迷宮を探索します。  

ただし、自由移動や自由視点操作は行わず、古典的な3DダンジョンRPGのように、1マスずつ移動し、90度ずつ回転します。



このテンプレートは、完成品のゲームではなく、Unity初心者が基本的なゲームループやスクリプト構成を学び、改造・写経・拡張するための教材用プロジェクトです。



\---



\## ゲーム概要



\- 3D空間に迷宮を生成する

\- プレイヤーは一人称視点で探索する

\- プレイヤーはグリッド単位で移動する

\- 自由移動・自由見回しは行わない

\- 壁のあるマスには進めない

\- ゴールマスに到達するとゲームクリアになる



\---



\## 操作方法



| キー | 操作 |

|---|---|

| W | 前進 |

| S | 後退 |

| A | 左に平行移動 |

| D | 右に平行移動 |

| Q | 左に90度回転 |

| E | 右に90度回転 |



A / D は向きを変えずに横移動します。  

Q / E はその場で向きを変える操作です。



\---



\## マップの作り方



このテンプレートでは、文字列でダンジョンマップを作成します。



使用する記号は以下の通りです。



| 記号 | 意味 |

|---|---|

| S | スタート地点 |

| G | ゴール |

| F | 床 |

| W | 壁 |



例：



```text

WWWWWWWW

WSFFFFFW

WFWWWFFW

WFFFWWGW

WWWWWWWW



マップ記号の意味

S：スタート



プレイヤーの開始位置です。

マップ内に1つ置くことを想定しています。



S

G：ゴール



プレイヤーがこのマスに到達すると、ゲームクリアになります。



G

F：床



プレイヤーが移動できる通常マスです。



F

W：壁



プレイヤーが移動できないマスです。

この位置には壁Prefabが配置されます。



W

必要なスクリプト



このテンプレートでは、以下のスクリプトを使用します。



DungeonMapData.cs

GridDirectionUtility.cs

DungeonBuilder.cs

GridPlayerController.cs

DungeonGameManager.cs

DungeonUIManager.cs

各スクリプトの役割

DungeonMapData.cs



ダンジョンの文字列マップを保存するためのScriptableObjectです。



マップをシーンに直接書くのではなく、アセットとして管理したい場合に使います。



例：



WWWWWWWW

WSFFFFFW

WFWWWFFW

WFFFWWGW

WWWWWWWW

GridDirectionUtility.cs



プレイヤーの向きや移動方向を管理するための補助スクリプトです。



このテンプレートでは、プレイヤーの向きを以下の4方向だけに制限しています。



North

East

South

West



このスクリプトは、以下のような処理を担当します。



North / East / South / West を移動量に変換する

左右90度回転後の向きを求める

前進・後退・左右移動の移動先を計算する

DungeonBuilder.cs



文字列マップを読み取り、3D空間にダンジョンを生成するスクリプトです。



主な役割は以下です。



文字列マップを読み取る

W の位置に WallPrefab を配置する

F、S、G の位置に FloorPrefab を配置する

G の位置に GoalPrefab を配置する

スタート地点を記録する

指定されたマスが歩けるかどうか判定する

指定されたマスがゴールかどうか判定する

GridPlayerController.cs



プレイヤーの移動と回転を管理するスクリプトです。



主な役割は以下です。



現在のグリッド座標を管理する

現在向いている方向を管理する

W / A / S / D / Q / E の入力を受け取る

1マス移動する

左右に90度回転する

壁のあるマスには進めないようにする

ゴール到達時に DungeonGameManager へ通知する



移動と回転は瞬間移動ではなく、Coroutineを使って短い時間をかけて行います。



DungeonGameManager.cs



ゲーム全体の流れを管理するスクリプトです。



主な役割は以下です。



ゲーム開始時にダンジョンを生成する

プレイヤーをスタート地点に配置する

ゴール到達時にゲームクリア処理を行う

ゴール後にプレイヤー操作を停止する

DungeonUIManager.cs



画面上のメッセージ表示を管理するスクリプトです。



主な役割は以下です。



操作説明を表示する

ゴール到達時に「迷宮を脱出した！」と表示する



TextMeshProUGUIを使う想定です。



Unityでのセットアップ手順

1\. スクリプトを配置する



以下のようなフォルダを作成し、スクリプトを入れます。



Assets

└── Scripts

&#x20;   └── Dungeon

&#x20;       ├── DungeonMapData.cs

&#x20;       ├── GridDirectionUtility.cs

&#x20;       ├── DungeonBuilder.cs

&#x20;       ├── GridPlayerController.cs

&#x20;       ├── DungeonGameManager.cs

&#x20;       └── DungeonUIManager.cs

2\. Prefabを用意する



最低限、以下の3つのPrefabを用意します。



FloorPrefab

WallPrefab

GoalPrefab



簡単には、Cubeを使って作成できます。



FloorPrefabの例



床用のPrefabです。



GameObject: Cube

Scale: X=4, Y=0.1, Z=4



薄い板のような形にします。



WallPrefabの例



壁用のPrefabです。



GameObject: Cube

Scale: X=4, Y=3, Z=4



床の上に立つ壁として使います。



壁の位置が床にめり込む場合は、DungeonBuilder の Wall Offset でY座標を調整してください。



GoalPrefabの例



ゴール地点を示すPrefabです。



例：



Cylinder

Sphere

光るPlane

旗のようなオブジェクト



床の上に置く目印として使います。



シーン構成



シーンには、最低限以下のGameObjectを用意します。



DungeonGameManager

DungeonBuilder

Main Camera

Canvas

DungeonGameManager



空のGameObjectを作成し、名前を DungeonGameManager にします。



アタッチするスクリプト：



DungeonGameManager.cs



Inspectorで以下を登録します。



項目	登録するもの

Dungeon Builder	DungeonBuilderオブジェクト

Player Controller	Main Camera

UI Manager	DungeonUIManagerを持つUIオブジェクト

DungeonBuilder



空のGameObjectを作成し、名前を DungeonBuilder にします。



アタッチするスクリプト：



DungeonBuilder.cs



Inspectorで以下を登録します。



項目	登録するもの

Floor Prefab	FloorPrefab

Wall Prefab	WallPrefab

Goal Prefab	GoalPrefab



Map Text には、ダンジョンの文字列マップを書きます。



例：



WWWWWWWW

WSFFFFFW

WFWWWFFW

WFFFWWGW

WWWWWWWW

Main Camera



Main Camera をプレイヤーとして使います。



アタッチするスクリプト：



GridPlayerController.cs



このカメラが、プレイヤーの一人称視点になります。



Eye Height を変更すると、カメラの高さを調整できます。



例：



Eye Height: 1.6

Canvas



UI表示用にCanvasを作ります。



構成例：



Canvas

└── MessagePanel

&#x20;   └── MessageText



MessageText は TextMeshProUGUI で作成してください。



CanvasまたはUI管理用の空オブジェクトに、以下のスクリプトをアタッチします。



DungeonUIManager.cs



Inspectorで以下を登録します。



項目	登録するもの

Message Root	MessagePanel

Message Text	MessageText

実行時の流れ



ゲームを再生すると、以下の順番で処理が行われます。



DungeonGameManagerが起動する

DungeonBuilderが文字列マップを読み取る

床、壁、ゴールのPrefabを3D空間に配置する

プレイヤーがスタート地点に配置される

プレイヤーがWASD/QEで移動・回転できるようになる

ゴールマスに到達すると「迷宮を脱出した！」と表示される

ゴール後は移動できなくなる

UnityのInput設定について



このテンプレートでは、初心者向けに旧Input Manager方式の入力を使用しています。



Input.GetKeyDown



もしキー入力が反応しない場合は、Unityの設定を確認してください。



Edit

→ Project Settings

→ Player

→ Active Input Handling



ここを以下のどちらかにしてください。



Both



または



Input Manager

よくあるトラブル

キーを押しても動かない



以下を確認してください。



GridPlayerController が Main Camera に付いているか

DungeonGameManager に Player Controller が登録されているか

Active Input Handling が Both または Input Manager になっているか

ゲームビューが選択されているか

壁をすり抜ける



以下を確認してください。



マップ上で壁にしたい場所が W になっているか

DungeonBuilder の Map Text が正しく書かれているか

DungeonBuilder に WallPrefab が登録されているか



このテンプレートでは、物理判定ではなく、グリッド座標によって壁判定を行っています。

そのため、Colliderがなくても壁判定自体は動きます。



プレイヤーの高さがおかしい



GridPlayerController の Eye Height を調整してください。



例：



Eye Height: 1.6

床や壁の位置がずれる



以下を確認してください。



DungeonBuilder の Cell Size

FloorPrefabのScale

WallPrefabのScale

Floor Offset

Wall Offset

Goal Offset



基本的には、Cell Size とPrefabの横幅・奥行きを合わせると扱いやすいです。



例：



Cell Size: 4

FloorPrefab Scale: X=4, Z=4

WallPrefab Scale: X=4, Z=4

初期版で実装していないもの



このテンプレートの初期版では、以下の機能は実装していません。



自動生成ダンジョン

ランダムエンカウント

戦闘

宝箱

鍵付き扉

ミニマップ

セーブ / ロード

自由移動

自由視点操作



まずは、固定マップ生成、グリッド移動、壁判定、ゴール判定だけを学ぶためのテンプレートです。



改造アイデア



慣れてきたら、以下のような改造ができます。



ミニマップを追加する



訪れたマスだけを記録し、画面上に小さなマップとして表示します。



必要になりそうな要素：



訪問済みマスの記録

現在位置の表示

向いている方向の表示

壁や床の表示

宝箱を追加する



新しいマップ記号を追加します。



例：



T = 宝箱



そして、DungeonBuilderで T を読み取って TreasurePrefab を配置します。



敵マスを追加する



新しいマップ記号を追加します。



例：



E = 敵



プレイヤーがそのマスに入ったら、戦闘画面に切り替えるようにできます。



鍵付き扉を追加する



新しいマップ記号を追加します。



例：



D = 扉

K = 鍵



鍵を持っていないと扉を通れない、という仕組みを追加できます。



マップをScriptableObjectで管理する



DungeonMapData を使うと、マップをアセットとして保存できます。



複数のマップを用意したい場合は、以下のような管理ができます。



Map\_Stage01

Map\_Stage02

Map\_Stage03

ライセンス・利用について



このテンプレートは、Unity初心者向けの学習・改造・写経用に作成したものです。



自由に改造して、以下のような用途に使えます。



Unity学習

授業や講座

サークル活動

ゲーム制作練習

3DダンジョンRPGの試作

まとめ



このテンプレートでは、3Dダンジョン探索ゲームの基本である、



固定マップ生成

床・壁・ゴールのPrefab配置

一人称視点

グリッド移動

90度回転

壁判定

ゴール判定



を学ぶことができます。



最初は小さなマップで動作を確認し、少しずつマップを広げたり、宝箱や敵、ミニマップなどを追加して改造してみてください。



