Imports System.Windows.Controls.Primitives
'Imports System.Windows.Threading
'Imports System.IO
'Imports System.ComponentModel
'Imports System.IO.Compression
'Imports System.Runtime.Serialization
'Imports System.Globalization

Class MainWindow
    Private MyPath As Path
    Private MyImage As Image
    Private MyLine As Line


    Private Shared Function IncrementOne(ByVal i As Integer) As Integer
        Return i + 1
    End Function
    Sub Mytest()

    End Sub
    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        AddHandler Button1.Click, AddressOf Button1_Click
        AddHandler Button2.Click, AddressOf Button2_Click
        AddHandler btnCheck1.Click, AddressOf btnCheck1_Click


        'MyCanvas.SnapsToDevicePixels = True
        'MyCanvas.UseLayoutRounding = False
        'Me.VisualBitmapScalingMode = BitmapScalingMode.NearestNeighbor


        Dim fa As New FrameworkElementFactory(GetType(Image), "TempImage")
        Dim ct As New ControlTemplate(GetType(Thumb))



        MyPath = New Path With {.Stroke = Brushes.Red, .StrokeThickness = 1}
        MyPath.Data = New RectangleGeometry(New Rect(0, 0, 50, 50))
        Canvas.SetLeft(MyPath, 100) : Canvas.SetTop(MyPath, 10)
        MyCanvas.Children.Add(MyPath)
        'MyPath.SnapsToDevicePixels = True
        'MyPath.UseLayoutRounding = True
        'RenderOptions.SetBitmapScalingMode(MyPath, BitmapScalingMode.NearestNeighbor)
        RenderOptions.SetEdgeMode(MyPath, EdgeMode.Aliased)

        Dim bi As New BitmapImage(New Uri("D:\ブログ用\テスト用画像\hueRect135.png"))
        MyImage = New Image
        MyImage.Source = bi
        Canvas.SetLeft(MyImage, 100) : Canvas.SetTop(MyImage, 100)
        MyCanvas.Children.Add(MyImage)

        MyLine = New Line
        With MyLine
            .Stroke = Brushes.Tomato
            .StrokeThickness = 1.0
            .X1 = 0 : .X2 = 100 : .Y1 = 0 : .Y2 = 50
        End With
        RenderOptions.SetBitmapScalingMode(MyLine, BitmapScalingMode.NearestNeighbor)
        RenderOptions.SetEdgeMode(MyLine, EdgeMode.Aliased)
        Canvas.SetLeft(MyLine, 210) : Canvas.SetTop(MyLine, 150)
        MyCanvas.Children.Add(MyLine)

        Call AddLine()

    End Sub
    Private Sub AddLine()
        Dim myL As New Line
        With myL
            .Stroke = Brushes.Tomato
            .StrokeThickness = 1.0
            .X1 = 0 : .X2 = 100 : .Y1 = 0 : .Y2 = 50
        End With
        MyStakcP1.Children.Add(myL)

    End Sub
    Private Sub CanvasToImageSave(target As FrameworkElement)
        Dim bmp As New RenderTargetBitmap(
            target.ActualWidth, target.ActualHeight, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(target)
        Dim encoder As New PngBitmapEncoder
        encoder.Frames.Add(BitmapFrame.Create(bmp))
        Using fs As New System.IO.FileStream("testImage.png", System.IO.FileMode.Create)
            encoder.Save(fs)
        End Using
    End Sub
    Private Sub ElemantToImageSave(target As FrameworkElement)
        'Dim r As Rect = GetRect(target)
        'Dim dv As New DrawingVisual
        'Using dc As DrawingContext = dv.RenderOpen
        '    Dim vb As New VisualBrush(MyCanvas)
        '    dc.DrawRectangle(vb, Nothing, r)
        'End Using
        'Dim rtb As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        'Dim bf As BitmapFrame = BitmapFrame.Create(rtb)
        Dim img As Image = target

        Dim bf As BitmapFrame = BitmapFrame.Create(img.Source)
        Dim encoder As New PngBitmapEncoder
        encoder.Frames.Add(BitmapFrame.Create(bf))
        Using fs As New System.IO.FileStream("testImage.png", System.IO.FileMode.Create)
            encoder.Save(fs)
        End Using
    End Sub
    Private Function GetRect(target As FrameworkElement) As Rect
        Dim gt As GeneralTransform = target.TransformToVisual(MyCanvas)
        Dim s As Size = New Size(target.ActualWidth, target.ActualHeight)
        Dim r As Rect = gt.TransformBounds(New Rect(s))
        Return r
    End Function
    Private Sub Button1_Click(sender As Object, e As RoutedEventArgs)
        Call CanvasToImageSave(MyCanvas)
    End Sub
    Private Sub Button2_Click(sender As Object, e As RoutedEventArgs)
        Call ElemantToImageSave(MyImage)
    End Sub

    Private Sub btnCheck1_Click(sender As Object, e As RoutedEventArgs)
        MsgBox($"{MyPath.ActualWidth}, {MyPath.ActualHeight}")

    End Sub
End Class
