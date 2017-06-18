Public Class Class1
    Inherits Border

    '依存プロパティ
    Public Shared ReadOnly Property MyAngleProperty As DependencyProperty =
        DependencyProperty.Register(NameOf(MyAngle),
                                    GetType(Double),
                                    GetType(Class1)) ', New PropertyMetadata(CDbl(0)))'要る？

    'Public Shared ReadOnly Property MyAngleProperty As DependencyProperty =
    '    DependencyProperty.Register(
    '    NameOf(MyAngle), GetType(Double), GetType(Class1), New PropertyMetadata(CDbl(0)))

    Public Property MyAngle As Double
        Get
            Return GetValue(MyAngleProperty)
        End Get
        Set(value As Double)
            SetValue(MyAngleProperty, value)
        End Set
    End Property

    'コンストラクタ、回転角度を初期値に指定できる
    Public Sub New(Optional angle As Double = 0)
        '各種TransformをグループにしてRenderTransformに指定
        Dim sc As New ScaleTransform '拡縮
        Dim sk As New SkewTransform 'ひし形
        Dim ro As New RotateTransform() '回転
        Dim tg As New TransformGroup
        With tg.Children 'transformグループ作成
            .Add(sc)
            .Add(sk)
            .Add(ro)
        End With
        Me.RenderTransform = tg '指定
        Me.RenderTransformOrigin = New Point(0.5, 0.5) '変形の基準点は中心
        MyAngle = angle '回転角度の初期値設定
        'MyAngle
        Dim str As String = "MyAngle"
        'バインディング
        '作成した依存プロパティのMyAnglePropertyをソースにして
        'RenderTransformの中のRotateTransformのAnglePropertyをターゲットにする場合
        Dim b As New Binding With {
            .Source = Me,
            .Path = New PropertyPath(MyAngleProperty),
            .Mode = BindingMode.TwoWay}
        BindingOperations.SetBinding(ro, RotateTransform.AngleProperty, b)

        '↑↓ソースとターゲットが入れ替わっているだけ

        'Dim b As New Binding With {
        '    .Source = ro,
        '    .Path = New PropertyPath(RotateTransform.AngleProperty),
        '    .Mode = BindingMode.TwoWay}
        'Me.SetBinding(MyAngleProperty, b)

    End Sub
End Class
