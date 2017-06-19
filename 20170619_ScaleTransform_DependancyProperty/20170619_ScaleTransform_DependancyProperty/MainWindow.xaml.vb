Imports System.Globalization

Class MainWindow
    Private ActBorder As Class1

    'バインディング作成用
    Private Function GetMyBinding(sObj As DependencyObject, sDp As DependencyProperty, strF As String) As Binding
        Dim b As New Binding With {
           .Source = sObj,
           .Path = New PropertyPath(sDp),
           .Mode = BindingMode.TwoWay,
           .StringFormat = strF}
        Return b
    End Function

    'ScaleXとScaleYを同期するチェックボックスをクリックしたとき
    Private Sub cbXY_Checked()
        If cbXY.IsChecked Then
            ActBorder.SetScaleLink(True)
        Else
            ActBorder.SetScaleLink(False)
        End If
    End Sub

    'アプリ起動時
    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        'Border作成してCanvasに追加
        ActBorder = New Class1() With {.Width = 30, .Height = 30, .Background = Brushes.Tomato}
        MyCanvas.Children.Add(ActBorder)
        Canvas.SetLeft(ActBorder, 100)
        Canvas.SetTop(ActBorder, 50)

        'バインディング設定
        'ソース：BorderのScale
        'ターゲット：スライダーとテキストブロック
        '作成
        Dim bindX As Binding = GetMyBinding(ActBorder, Class1.MyScaleXProperty, "ScaleX = {0:0.0}")
        Dim bindY As Binding = GetMyBinding(ActBorder, Class1.MyScaleYProperty, "ScaleY = {0:0.0}")
        'ターゲットにバインディング
        sldXscale.SetBinding(Slider.ValueProperty, bindX)
        sldYscale.SetBinding(Slider.ValueProperty, bindY)
        tbScaleX.SetBinding(TextBlock.TextProperty, bindX)
        tbScaleY.SetBinding(TextBlock.TextProperty, bindY)

        'チェックボックスとボタンのクリックイベントと動かす関数を関連付け
        AddHandler cbXY.Click, AddressOf cbXY_Checked '同期の有無
        AddHandler btXadd.Click, AddressOf ScaleXAdd '拡大
        AddHandler btXsub.Click, AddressOf ScaleXSub '縮小
    End Sub

    Private Sub ScaleXAdd()
        ActBorder.MyScaleX += 1
    End Sub
    Private Sub ScaleXSub()
        ActBorder.MyScaleX -= 1
    End Sub
End Class



'-----



Public Class Class1
    Inherits Border 'Borderクラスをを継承
    Private MyScaleTransform As ScaleTransform

    '依存関係プロパティ
    'ScaleX用
    Public Shared ReadOnly Property MyScaleXProperty As DependencyProperty =
        DependencyProperty.Register(NameOf(MyScaleX), GetType(Double), GetType(Class1), New PropertyMetadata(1.0))
    Public Property MyScaleX As Double
        Get
            Return GetValue(MyScaleXProperty)
        End Get
        Set(value As Double)
            SetValue(MyScaleXProperty, value)
        End Set
    End Property

    'ScaleY用
    Public Shared ReadOnly Property MyScaleYProperty As DependencyProperty =
        DependencyProperty.Register(NameOf(MyScaleY), GetType(Double), GetType(Class1), New PropertyMetadata(1.0))
    Public Property MyScaleY As Double
        Get
            Return GetValue(MyScaleYProperty)
        End Get
        Set(value As Double)
            SetValue(MyScaleYProperty, value)
        End Set
    End Property

    'バインディングソースの作成用
    Private Function GetMyBinding(sObj As DependencyObject, sDp As DependencyProperty) As Binding
        Dim b As New Binding With {
            .Source = sObj,
            .Path = New PropertyPath(sDp),
            .Mode = BindingMode.TwoWay}
        Return b
    End Function

    'コンストラクタ
    Public Sub New()
        Call Me.New(1.0, 1.0)
    End Sub
    Public Sub New(sx As Double, sy As Double)
        MyScaleX = sx
        MyScaleY = sy
        '各種トランスフォームをグループにしてRenderTransformに指定
        MyScaleTransform = New ScaleTransform '拡縮、今回のメイン
        Dim sk As New SkewTransform '並行変形、今回は未使用
        Dim ro As New RotateTransform '回転、今回は未使用
        'グループ作成
        Dim tg As New TransformGroup
        With tg.Children
            .Add(MyScaleTransform) : .Add(sk) : .Add(ro)
        End With
        Me.RenderTransform = tg '指定
        Me.RenderTransformOrigin = New Point(0.5, 0.5) '変形の基準点は中心

        'バインディング
        'ソース：用意した依存関係プロパティ
        'ターゲット：ScaleTransform
        BindingOperations.SetBinding(MyScaleTransform, ScaleTransform.ScaleXProperty, GetMyBinding(Me, MyScaleXProperty))
        BindingOperations.SetBinding(MyScaleTransform, ScaleTransform.ScaleYProperty, GetMyBinding(Me, MyScaleYProperty))
    End Sub

    'ScaleXとScaleYを同期するかしないかの切り替え
    Public Sub SetScaleLink(IsLink As Boolean)
        If IsLink Then '同期する場合
            'Xをソースにして、Yをターゲットにする
            BindingOperations.SetBinding(Me, MyScaleYProperty, GetMyBinding(Me, Class1.MyScaleXProperty))

        Else '同期しない場合(別々に戻す)
            'Yのバインディングを外す(空のBindingをバインディングする)
            BindingOperations.SetBinding(Me, MyScaleYProperty, New Binding)
            '今の値を継続したいのでXの値をYにコピー、これをしないとYの値が初期値の1になってしまう
            MyScaleY = MyScaleX
        End If
    End Sub
End Class


'未使用
Public Class ConverterPointToDouble
    Implements System.Windows.Data.IMultiValueConverter

    Public Function Convert(values() As Object,
                            targetType As Type,
                            parameter As Object,
                            culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
        'Throw New NotImplementedException()
        'X,YをPointに変換
        Dim p As New Point(values(0), values(1))
        Return p
    End Function

    Public Function ConvertBack(value As Object,
                                targetTypes() As Type,
                                parameter As Object,
                                culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotImplementedException()
        'PointからXとYに変換
        Dim p As Point = DirectCast(value, Point)
        Dim o() As Object = New Object() {p.X, p.Y}
        Return o
    End Function
End Class