'WPFとVB.NET、ControlTemplateを使ったThumbを回転表示する時に回転させるのはどれがいいのか ( ソフトウェア ) - 午後わてんのブログ - Yahoo!ブログ
'https://blogs.yahoo.co.jp/gogowaten/14157487.html
'Window.Resourcesの、C#での記述が分かりません
'https://social.msdn.microsoft.com/Forums/silverlight/ja-JP/08759383-00cc-4891-bf05-7755ad9e01dc/windowresourcesc?forum=wpfja


Imports System.Windows.Controls.Primitives
'Imports System.Windows.Threading

Class MainWindow
    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        Dim fa As New FrameworkElementFactory
        fa.Type = GetType(Border)
        fa.Name = "TempBorder"
        fa.SetValue(Border.BackgroundProperty, Brushes.Transparent)

        Dim ct As New ControlTemplate
        ct.TargetType = GetType(Thumb)
        ct.VisualTree = fa

        Dim t As New Thumb
        With t
            .Template = ct
            .Width = 50
            .Height = 50
            '.Background = Brushes.Transparent
        End With
        Canvas.SetLeft(t, 100)
        Canvas.SetTop(t, 100)
        AddHandler t.DragDelta, AddressOf ThumbDrag
        MyCanvas.Children.Add(t)
        t.ApplyTemplate() '必須

        Dim str = ct.VisualTree.Name
        Dim ti0 As Border = ct.FindName("TempBorder", t)
        Dim ti1 As Border = ct.FindName((ct.VisualTree.Name), t)
        Dim ti2 As Border = DirectCast(ct.FindName((ct.VisualTree.Name), t), Border)
        ti2.BorderBrush = Brushes.Aqua
        ti2.BorderThickness = New Thickness(1)
        'ti2.Background = Brushes.Transparent

        Dim fa2 As New FrameworkElementFactory(GetType(StackPanel), "TempStackP")
        Dim fa3 As New FrameworkElementFactory(GetType(Border), "TempBorder")
        Dim fa4 As New FrameworkElementFactory(GetType(TextBlock), "TempTextBlock")
        fa2.SetValue(StackPanel.BackgroundProperty, Brushes.LightBlue)
        fa3.SetValue(Border.BackgroundProperty, Brushes.Blue)
        fa3.SetValue(WidthProperty, 50.0)
        fa3.SetValue(HeightProperty, 25.0)
        fa4.SetValue(TextBlock.TextProperty, "temp")
        fa2.AppendChild(fa3)
        fa2.AppendChild(fa4)
        Dim ct2 As New ControlTemplate(GetType(Thumb))
        ct2.VisualTree = fa2
        Dim t2 As New Thumb
        With t2
            .Template = ct2
            .Width = 50
            .Height = 50
        End With
        Canvas.SetLeft(t2, 100)
        Canvas.SetTop(t2, 50)
        AddHandler t2.DragDelta, AddressOf ThumbDrag
        MyCanvas.Children.Add(t2)

    End Sub

    Private Sub ThumbDrag(t As Thumb, e As DragDeltaEventArgs)
        Canvas.SetLeft(t, Canvas.GetLeft(t) + e.HorizontalChange)
        Canvas.SetTop(t, Canvas.GetTop(t) + e.VerticalChange)

    End Sub
End Class
