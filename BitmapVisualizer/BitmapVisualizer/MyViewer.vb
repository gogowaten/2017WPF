Imports System.Drawing
Imports System.Windows.Forms

Public Class MyViewer
    Public BaseImage As Image

    Public Sub New()

        ' この呼び出しはデザイナーで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後で初期化を追加します。
        'Me.PictureBox1.Image = bmp

    End Sub
    Public Sub SetBitmap(bmp As Bitmap)
        BaseImage = bmp
    End Sub

    'Private Sub PictureBox1_Paint(sender As Object, e As PaintEventArgs) Handles PictureBox1.Paint
    '    e.Graphics.DrawImage(BaseImage, New Point(0, 0))
    'End Sub
End Class