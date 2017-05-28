Class MainWindow
    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        Dim bmp As New BitmapImage(New Uri("D:\ブログ用\チェック用2\NEC_6506_2016_11_18_午後わてん.jpg"))
        MyImage.Source = bmp


    End Sub
End Class
