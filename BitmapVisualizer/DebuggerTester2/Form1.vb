Public Class Form1


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim bmp As Bitmap = PictureBox1.Image
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim bmp As New Bitmap("D:\ブログ用\チェック用2\NEC_6507_2016_11_18_午後わてん.jpg")
        Me.PictureBox1.Image = bmp

        'BitmapVisualizer.DebuggerTest.TestShowVisualizer(bmp)


    End Sub
End Class
