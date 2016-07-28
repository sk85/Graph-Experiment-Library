# Cubes
<p>ハイパーキューブ系列のグラフの実験に使えるコード群</p>

##更新
・古いソリューションファイルが嫌だったので、新しく作り変えてレポジトリもすっきり一新しました<br>
・任意の位相に対応しました


##クラス
<ul>
  <li>SGraph</li>
  キューブ系クラス用の基本クラス
  <li>Experiment</li>
  実験パラメータを格納するクラス
</ul>

<h2>コード類</h2>
namespaceで整頓
<ul>
  <li>
    Routing</br>
    ルーティング系の関数が定義されている
  </li>
  <li>
    Sample</br>
    実験のソースコードのサンプル
  </li>
</ul>

<h2>使い方</h2>
<ol>
  <li>
    SGraphを継承して好きなグラフのクラスを作る
  </li>
  <li>
    Sampleのコードを見ながら適当にプログラミング
  </li>
  <li>
    VS2015u3で開発をしています。GCCだとヘッダを書き換えないとまずい？<br>
    プロジェクトにパスを通したりしてあるので、VSでの利用を推奨します。
  </li>
</ol>