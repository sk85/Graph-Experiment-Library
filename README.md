# Graph-Experiment-Library
グラフに関する実験がいろいろできるライブラリ。
VS2017で開発中。
* GraphExperimentLibrary : はじめに作ったC++用実験環境。SQの最短経路選択はここでしか実装してない
* GraphExperimentLibraryForCS : C#向けに移植を目指していたもの。
* GraphCS : *現在開発中のC#向け実験環境。*一通り必要な機能は実装済み。

## 更新
<table>
 <tr>
  <td>以前</td>
  <td>プロジェクトが2世代くらい変わったので全部消した</td>
 </tr>
 <tr>
  <td>2017/10/25</td>
  <td>GraphCSに一通りの機能を実装</td>
 </tr>
</table>

## 使い方
Hypercubeのクラスを作る場合を考えます。

### ノードのクラスを作る
ノードを表現するクラスを、ANodeクラスを継承して作ります。
AGraphは抽象クラスなので幾つかのメンバを実装しましょう。

    Class BinaryNode : ANode
    {
        // アドレスを0で初期化するコンストラクタ
        public BinaryNode() : base() { }
        
        // アドレスを指定して初期化するコンストラクタ
        public BinaryNode(int addr) : base(addr) { }
        
        // アドレスの実体
        // 今回はビット列なのでint型で十分
        public override int Addr { get; set; }
    }

### グラフのクラスを作る
次に、Hypercubeクラスを、AGraph<NodeType>クラスを継承して作ります。
NodeTypeはANodeの子孫クラスでなければなりません。
先程作ったBinaryNodeを利用しましょう。
また、こちらも抽象クラスなので幾つかの抽象メンバを実装しましょう。

    class Hypercube : AGraph<BinaryNode>
    {
        // コンストラクタ。dimは次元数
        public Hypercube(int dim) : base(dim) { }
        
        // グラフの名前
        public override string Name => "Hypercube";
        
        // ノード数を計算
        // HQのノード数は2^(次元数)なので以下の式
        protected override int CalcNodeNum() => 1 << Dimension;
        
        // ノードの次数を返す
        // n-HQはn正則なので常に次元数を返す
        public override int GetDegree(BinaryNode node) => Dimension;
        
        // nodeの第i隣接頂点を返す
        // HQは第iビットのみ反転して返せばよい
        public override BinaryNode GetNeighbor(BinaryNode node, int i) => node ^ (1 << i);
    }

### 自由に実験を行う
Experimentクラスは実験のパラメタなどを用意するクラスです。
指定した故障率で故障を発生させたり、その中で非故障な連結ペアを見つけたりする機能が用意されています。

    // ノードの型とグラフ、乱数のシード値を指定してインスタンス化
    // 今回は10次元HQ
    var exp = new Experiment<BinaryNode>(new HyperCube(10), 0);
    
    // 故障率を5%に指定
    exp.FaultRatio = 0.05
    
    // 実験パラメタを更新
    // 故障状態(FaultFlags)、出発頂点(SourceNode)、目的頂点(DestinationNode)が更新される
    exp.Next();
    
    // ステップ数の上限を次元数に指定してルーティング実行
    // 結果が非負なら成功でステップ数、-1なら袋小路に当たって失敗、-2ならタイムアウト
    int step = exp.Routing_170920(exp.G.Dimension)

このようにたった4行あればルーティングの実験が可能です。
このルーティングでは頂点間の距離を計算していますが、`AGraph.CalcDistanceBFS`という幅優先探索を利用したメソッドが呼ばれます。
このためちょっと遅いので、`Hypercube.CalcDistance`をオーバーライドして、もっと効率的に距離を計算するコードに書き換えることができます。

HypercubeクラスとBinaryNodeクラスはすでに実装済みのものがあるので参考にしてください。
また、`Test.Test171024`にルーティングの実験の痕跡があるので参考にしてください。
