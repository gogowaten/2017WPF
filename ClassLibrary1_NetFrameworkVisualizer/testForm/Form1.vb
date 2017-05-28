Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim bmp As New Bitmap("D:\ブログ用\チェック用2\NEC_6506_2016_11_18_午後わてん.jpg")
        PictureBox1.Image = bmp

    End Sub
End Class
