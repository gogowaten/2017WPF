Imports System.Runtime.InteropServices

Class MainWindow
    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized


        Dim img As BitmapImage
        img = New BitmapImage(New Uri("D:\ブログ用\テスト用画像\NEC_8042_2017_05_09_午後わてん_.jpg"))
        'img = New BitmapImage(New Uri(uriString:="D:\ブログ用\テスト用画像\10x10gray.bmp"))
        'img = New BitmapImage(New Uri(uriString:="D:\ブログ用\テスト用画像\3x3gray.bmp"))
        img = New BitmapImage(New Uri("D:\ブログ用\テスト用画像\NEC_8041_2017_05_09_午後わてん_.jpg"))
        img = New BitmapImage(New Uri("D:\ブログ用\テスト用画像\青空とトマトの花.jpg"))

        'img = New BitmapImage(New Uri("D:\ブログ用\テスト用画像\kinngyo.bmp"))
        'img = New BitmapImage(New Uri("D:\ブログ用\テスト用画像\セセリチョウ_NEC_0625_.png"))
        'img = New BitmapImage(New Uri("D:\ブログ用\テスト用画像\セセリチョウ_NEC_0625_格子.png"))

        'img = New BitmapImage(New Uri("D:\ブログ用\テスト用画像\NEC_9017_2015_07_23_300ml_.jpg"))
        'img = New BitmapImage(New Uri("D:\ブログ用\テスト用画像\grayScale.bmp"))
        'img = New BitmapImage(New Uri("D:\ブログ用\テスト用画像\gray240.bmp"))
        'img = New BitmapImage(New Uri("D:\ブログ用\テスト用画像\8x8gray240.bmp"))
        'img = New BitmapImage(New Uri("D:\ブログ用\テスト用画像\Gray240_255x255.bmp"))
        'img = New BitmapImage(New Uri("D:\ブログ用\テスト用画像\Gray240and90_255x255.bmp"))


        'img = New BitmapImage(New Uri("D:\ブログ用\エクセルVBA\test.bmp"))
        'img = New BitmapImage(New Uri("D:\ブログ用\テスト用画像\Michelangelo's_David_-_63_grijswaarden.bmp"))

        'img = New BitmapImage(New Uri("D:\ブログ用\エクセルVBA\testLena.bmp"))

        'Dim ore = img1.Stretch
        'img1.Stretch = Stretch.UniformToFill

        img1.Source = img

        '8色
        img2.Source = SubtractiveColor8減色(img, 128)

        img3.Source = Ditering2x2Color2(img)
        Call SaveImage(img3.Source, "D:\ブログ用\テスト用画像\2x2Color8.bmp")
        img4.Source = Ditering4x4Color8(img)
        Call SaveImage(img4.Source, "D:\ブログ用\テスト用画像\4x4Color8.bmp")
        '誤差拡散法、8色
        img5.Source = FroydSteinberg8Color誤差拡散法(img)



        Dim wb As New WriteableBitmap(img)
        Dim h As Integer = wb.PixelHeight
        Dim w As Integer = wb.PixelWidth

        Dim stride As Integer = wb.BackBufferStride
        Dim pixels(h * stride - 1) As Byte
        Dim p As Integer
        wb.CopyPixels(pixels, stride, 0)

        'グレースケール化
        Dim g As Integer
        For y As Integer = 0 To h - 1
            For x As Integer = 0 To w - 1
                p = y * stride + (x * 4)
                g = CInt(CInt(pixels(p)) + CInt(pixels(p + 1)) + CInt(pixels(p + 2))) / 3
                pixels(p + 0) = g ' col.B
                pixels(p + 1) = g ' col.G
                pixels(p + 2) = g ' col.R
                pixels(p + 3) = 255
            Next
        Next

        'グレースケール表示
        wb.WritePixels(New Int32Rect(0, 0, w, h), pixels, stride, 0)
        img6.Source = wb
        '単純2色
        img7.Source = GrayScale2Sirokuro(img)
        'パターンディザ、グレースケールを白黒2色
        img8.Source = Ditering2x2GrayScale2(wb)
        img9.Source = Ditering4x4GrayScale2(wb)
        Call SaveImage(img9.Source, "D:\ブログ用\テスト用画像\4x4GrayScale2.bmp")
        '白黒2色、誤差拡散法
        img10.Source = FroydSteinberg誤差拡散法(wb)

        'img11.Source = Ditering2x2GrayScale(wb)
        'img12.Source = Ditering4x4GrayScale(wb)
        'img13.Source = SubtractiveColor8減色(img, 100)




    End Sub


    Private Function FroydSteinberg誤差拡散法(img As BitmapSource) As BitmapSource
        Dim wb As New WriteableBitmap(img)
        Dim h As Integer = wb.PixelHeight
        Dim w As Integer = wb.PixelWidth

        Dim stride As Integer = wb.BackBufferStride
        Dim pixels(h * stride - 1) As Byte
        Dim p As Long
        wb.CopyPixels(pixels, stride, 0)

        Dim carryOver As Integer
        Dim diffP As Long
        Dim v As Double
        For y As Integer = 0 To h - 1
            For x As Integer = 0 To w - 1
                p = y * stride + (x * 4)
                carryOver = pixels(p)
                If pixels(p) > 128 Then
                    '白
                    pixels(p + 0) = 255 ' col.B
                    pixels(p + 1) = 255 ' col.G
                    pixels(p + 2) = 255 ' col.R
                    carryOver -= 255
                Else
                    '黒
                    pixels(p + 0) = 0 ' col.B
                    pixels(p + 1) = 0 ' col.G
                    pixels(p + 2) = 0 ' col.R
                End If

                '誤差拡散
                v = carryOver / 16
                If x <> w - 1 Then
                    'pixels(p + 4) += v * 7 '右
                    pixels(p + 4) = GetNormalizationByte(v * 7, pixels(p + 4))
                End If


                If y < h - 1 Then
                    '下段
                    diffP = p + stride
                    '真下
                    'pixels(diffP) += v * 5
                    pixels(diffP) = GetNormalizationByte(v * 5, pixels(diffP))

                    If x <> w - 1 Then
                        '右下
                        pixels(diffP + 4) = GetNormalizationByte(v * 1, pixels(diffP + 4))
                    End If

                    If x <> 0 Then
                        '左下
                        ''pixels(diffP - 4) += v * 3
                        pixels(diffP - 4) = GetNormalizationByte(v * 3, pixels(diffP - 4))
                    End If
                End If

            Next
        Next

        Dim sourceRect As Int32Rect = New Int32Rect(0, 0, w, h)
        wb.WritePixels(sourceRect, pixels, stride, 0)
        Return wb

    End Function

    Private Function GetNormalizationByte(gosa As Integer, b As Byte) As Byte
        Dim v As Integer = gosa + b
        Dim r As Byte
        If v < 0 Then
            r = 0
        ElseIf v > 255 Then
            r = 255
        Else
            r = v
        End If
        Return r
    End Function

    Private Sub SetColorFroydSteinberg(p As Long, pixels() As Byte, x As Integer, y As Integer, w As Integer, h As Integer, stride As Long)
        Dim carryByte As Integer = pixels(p) '誤差

        If pixels(p) > 128 Then
            pixels(p) = 255
            carryByte -= 255
        Else
            pixels(p) = 0
        End If

        '誤差最小単位
        Dim v As Double
        v = carryByte / 16

        '右ピクセル
        If x <> w - 1 Then
            'pixels(p + 4) += v * 7 '右
            SetNormalizationByte(pixels, p + 4, v * 7)
        End If

        '下段
        If y < h - 1 Then
            '真下
            ''pixels(diffP) += v * 5
            Call SetNormalizationByte(pixels, p + stride, v * 5)
            If x <> w - 1 Then
                '右下
                Call SetNormalizationByte(pixels, p + stride + 4, v * 1)
            End If

            If x <> 0 Then
                '左下
                ''pixels(diffP - 4) += v * 3
                Call SetNormalizationByte(pixels, p + stride - 4, v * 3)
            End If
        End If
    End Sub
    '誤差拡散法で白黒赤緑青黄色水色マゼンタの8色に減色
    Private Function FroydSteinberg8Color誤差拡散法(img As BitmapSource) As BitmapSource
        Dim wb As New WriteableBitmap(img)
        Dim h As Integer = wb.PixelHeight
        Dim w As Integer = wb.PixelWidth

        Dim stride As Integer = wb.BackBufferStride
        Dim pixels(h * stride - 1) As Byte
        Dim p As Long

        wb.CopyPixels(pixels, stride, 0)

        For y As Integer = 0 To h - 1
            For x As Integer = 0 To w - 1
                p = y * stride + (x * 4)
                Call SetColorFroydSteinberg(p, pixels, x, y, w, h, stride)
                Call SetColorFroydSteinberg(p + 1, pixels, x, y, w, h, stride)
                Call SetColorFroydSteinberg(p + 2, pixels, x, y, w, h, stride)
                pixels(p + 3) = 255
            Next
        Next

        wb.WritePixels(New Int32Rect(0, 0, w, h), pixels, stride, 0)
        Return wb
    End Function

    'RGB各値と誤差値を足してByteの0から255に収める
    '0以下の場合は0、255以上なら255にする
    Private Sub SetNormalizationByteRGB(pixels() As Byte, p As Long, gosa As Double)
        Dim v As Integer
        For i As Integer = 0 To 2
            v = pixels(p + i) + gosa
            If v < 0 Then
                v = 0
            ElseIf v > 255 Then
                v = 255
            End If
            pixels(p + i) = v
        Next
    End Sub

    Private Sub SetNormalizationByte(pixels() As Byte, p As Long, gosa As Double)
        Dim v As Integer
        v = pixels(p) + gosa
        If v < 0 Then
            v = 0
        ElseIf v > 255 Then
            v = 255
        End If
        pixels(p) = v

    End Sub

    '普通の8色へ減色、しきい値thresholdは128が普通
    Private Function SubtractiveColor8減色(img As BitmapSource, threshold As Integer) As BitmapSource
        Dim wb As New WriteableBitmap(img)
        Dim h As Integer = wb.PixelHeight
        Dim w As Integer = wb.PixelWidth

        Dim stride As Integer = wb.BackBufferStride
        Dim pixels(h * stride - 1) As Byte
        Dim p As Long

        wb.CopyPixels(pixels, stride, 0)

        For y As Integer = 0 To h - 1
            For x As Integer = 0 To w - 1
                p = y * stride + (x * 4)
                For cnt As Integer = 0 To 2
                    If pixels(p + cnt) > threshold Then
                        pixels(p + cnt) = 255
                    Else
                        pixels(p + cnt) = 0
                    End If
                Next
                pixels(p + 3) = 255 '透明度、これを指定しないと色が微妙に変化する？
            Next
        Next


        wb.WritePixels(New Int32Rect(0, 0, w, h), pixels, stride, 0)
        Return wb
    End Function


    '微妙な失敗
    '2x2のパターンディザ、グレースケール画像を白黒の2色
    '2x2の平均色を元にパターンを決定するのでぼやけるし縦横ピクセルが偶数の画像に限る
    Private Function Ditering2x2GrayScale(img As BitmapSource) As BitmapSource
        Dim wb As New WriteableBitmap(img)
        Dim h As Integer = wb.PixelHeight
        Dim w As Integer = wb.PixelWidth
        Dim stride As Integer = wb.BackBufferStride
        Dim pixels(h * stride - 1) As Byte
        Dim p As Long, p2 As Long
        Dim colorAvg As Double

        wb.CopyPixels(pixels, stride, 0)

        For y As Integer = 0 To h - 2 Step 2
            For x As Integer = 0 To w - 2 Step 2
                p = y * stride + (x * 4) '基準セルの場所
                p2 = p + stride '基準セルの一行下のセルの場所
                colorAvg = (CInt(pixels(p) + CInt(pixels(p + 4)) + CInt(pixels(p2)) + CInt(pixels(p2 + 4)))) / 4
                '4セルすべてを白で塗る
                Call SetColorGray(pixels, p, 255)
                Call SetColorGray(pixels, p + 4, 255)
                Call SetColorGray(pixels, p2, 255)
                Call SetColorGray(pixels, p2 + 4, 255)
                '必要に応じて黒に塗る
                If colorAvg <= 204 Then Call SetColorGray(pixels, p, 0) '左上セル
                If colorAvg <= 153 Then Call SetColorGray(pixels, p2 + 4, 0) '右下
                If colorAvg <= 102 Then Call SetColorGray(pixels, p + 4, 0) '右上
                If colorAvg <= 51 Then Call SetColorGray(pixels, p2, 0) '左下

            Next
        Next

        wb.WritePixels(New Int32Rect(0, 0, w, h), pixels, stride, 0)
        Return wb
    End Function

    Private Sub SetColorGray(pixels() As Byte, p As Long, g As Byte)
        For i As Integer = 0 To 2
            pixels(p + i) = g
        Next
    End Sub

    '微妙な失敗
    '4x4の平均色からパターンに当てはめていく
    Private Function Ditering4x4GrayScale(img As BitmapSource) As BitmapSource
        Dim wb As New WriteableBitmap(img)
        Dim h As Integer = wb.PixelHeight
        Dim w As Integer = wb.PixelWidth
        Dim i, j As Integer
        Dim stride As Integer = wb.BackBufferStride
        Dim pixels(h * stride - 1) As Byte
        Dim p As Long
        Dim colorAvg As Double
        wb.CopyPixels(pixels, stride, 0)

        For y As Integer = 0 To h - 4 Step 4
            For x As Integer = 0 To w - 4 Step 4
                colorAvg = 0
                p = y * stride + (x * 4) '基準セルの場所
                For i = 0 To 3
                    colorAvg += CInt(pixels(p + i * 4) +
                        CInt(pixels(p + i * 4 + stride))) +
                        CInt(pixels(p + i * 4 + stride * 2)) +
                        CInt(pixels(p + i * 4 + stride * 3))
                Next
                colorAvg /= 16


                '16セルすべてを白で塗る
                For i = 0 To 3
                    For j = 0 To 3
                        Call SetColorGray(pixels, (p + i * 4) + (stride * j), 255)
                    Next
                Next

                '必要に応じて黒に塗る
                If colorAvg <= 240 Then Call SetColorGray(pixels, p + 0, 0) '0,左上セル
                If colorAvg <= 225 Then Call SetColorGray(pixels, p + 8 + stride * 2, 0) '1
                If colorAvg <= 210 Then Call SetColorGray(pixels, p + 8, 0) '2
                If colorAvg <= 195 Then Call SetColorGray(pixels, p + 0 + stride * 2, 0) '3
                If colorAvg <= 180 Then Call SetColorGray(pixels, p + 4 + stride, 0) '4
                If colorAvg <= 165 Then Call SetColorGray(pixels, p + 12 + stride * 3, 0) '5
                If colorAvg <= 150 Then Call SetColorGray(pixels, p + 12 + stride, 0) '6
                If colorAvg <= 135 Then Call SetColorGray(pixels, p + 4 + stride * 3, 0) '7
                If colorAvg <= 120 Then Call SetColorGray(pixels, p + 4, 0) '8
                If colorAvg <= 105 Then Call SetColorGray(pixels, p + 12 + stride * 2, 0) '9
                If colorAvg <= 90 Then Call SetColorGray(pixels, p + 12, 0) '10
                If colorAvg <= 75 Then Call SetColorGray(pixels, p + 4 + stride * 2, 0) '11
                If colorAvg <= 60 Then Call SetColorGray(pixels, p + 0 + stride, 0) '12
                If colorAvg <= 45 Then Call SetColorGray(pixels, p + 8 + stride * 3, 0) '13
                If colorAvg <= 30 Then Call SetColorGray(pixels, p + 8 + stride, 0) '14
                If colorAvg <= 15 Then Call SetColorGray(pixels, p + 0 + stride * 3, 0) '15

            Next
        Next
        wb.WritePixels(New Int32Rect(0, 0, w, h), pixels, stride, 0)
        Return wb
    End Function


    '正解のパターンディザ4x4
    Private Function Ditering4x4GrayScale2(img As BitmapSource) As BitmapSource
        Dim thresholdMap As Single()() = New Single(3)() {
            New Single(3) {1.0F / 17.0F, 9.0F / 17.0F, 3.0F / 17.0F, 11.0F / 17.0F},
            New Single(3) {13.0F / 17.0F, 5.0F / 17.0F, 15.0F / 17.0F, 7.0F / 17.0F},
            New Single(3) {4.0F / 17.0F, 12.0F / 17.0F, 2.0F / 17.0F, 10.0F / 17.0F},
            New Single(3) {16.0F / 17.0F, 8.0F / 17.0F, 14.0F / 17.0F, 6.0F / 17.0F}
        }
        Dim wb As New WriteableBitmap(img)
        Dim h As Integer = wb.PixelHeight
        Dim w As Integer = wb.PixelWidth
        Dim stride As Integer = wb.BackBufferStride
        Dim pixels(h * stride - 1) As Byte
        Dim p As Long
        wb.CopyPixels(pixels, stride, 0)

        For y As Integer = 0 To h - 1
            For x As Integer = 0 To w - 1
                p = y * stride + (x * 4) '基準セルの場所
                If thresholdMap(x Mod 4)(y Mod 4) <= pixels(p) / 255 Then
                    SetColorGray(pixels, p, 255)
                Else
                    SetColorGray(pixels, p, 0)
                End If
            Next
        Next
        wb.WritePixels(New Int32Rect(0, 0, w, h), pixels, stride, 0)
        Return wb
    End Function

    'カラー版パターンディザの4x4
    Private Function Ditering4x4Color8(img As BitmapSource) As BitmapSource
        Dim thresholdMap As Single()() = New Single(3)() {
            New Single(3) {1.0F / 17.0F, 9.0F / 17.0F, 3.0F / 17.0F, 11.0F / 17.0F},
            New Single(3) {13.0F / 17.0F, 5.0F / 17.0F, 15.0F / 17.0F, 7.0F / 17.0F},
            New Single(3) {4.0F / 17.0F, 12.0F / 17.0F, 2.0F / 17.0F, 10.0F / 17.0F},
            New Single(3) {16.0F / 17.0F, 8.0F / 17.0F, 14.0F / 17.0F, 6.0F / 17.0F}
        }
        Dim threshold As Single
        Dim wb As New WriteableBitmap(img)
        Dim h As Integer = wb.PixelHeight
        Dim w As Integer = wb.PixelWidth
        Dim stride As Integer = wb.BackBufferStride
        Dim pixels(h * stride - 1) As Byte
        Dim p As Long
        wb.CopyPixels(pixels, stride, 0)

        For y As Integer = 0 To h - 1
            For x As Integer = 0 To w - 1
                threshold = thresholdMap(x Mod 4)(y Mod 4)
                p = y * stride + (x * 4) '基準セルの場所
                Call SetColor(threshold, pixels, p)
            Next
        Next
        wb.WritePixels(New Int32Rect(0, 0, w, h), pixels, stride, 0)
        Return wb
    End Function

    'RGB各色をしきい値と比較して0か255にする
    Private Sub SetColor(threshold As Single, pixels() As Byte, p As Long)
        For i As Integer = 0 To 2
            If threshold <= pixels(p + i) / 255 Then
                pixels(p + i) = 255
            Else
                pixels(p + i) = 0
            End If
        Next
    End Sub

    'パターンディザ2x2、正解版
    Private Function Ditering2x2GrayScale2(img As BitmapSource) As BitmapSource
        Dim thresholdMap As Single()() = New Single(1)() {
            New Single(1) {1.0F / 5.0F, 3.0F / 5.0F},
            New Single(1) {4.0F / 5.0F, 2.0F / 5.0F}
        }
        Dim wb As New WriteableBitmap(img)
        Dim h As Integer = wb.PixelHeight
        Dim w As Integer = wb.PixelWidth
        Dim stride As Integer = wb.BackBufferStride
        Dim pixels(h * stride - 1) As Byte
        Dim p As Long
        wb.CopyPixels(pixels, stride, 0)

        For y As Integer = 0 To h - 1
            For x As Integer = 0 To w - 1
                p = y * stride + (x * 4) '基準セルの場所
                If thresholdMap(x Mod 2)(y Mod 2) <= pixels(p) / 255 Then
                    SetColorGray(pixels, p, 255)
                Else
                    SetColorGray(pixels, p, 0)
                End If
            Next
        Next
        wb.WritePixels(New Int32Rect(0, 0, w, h), pixels, stride, 0)
        Return wb
    End Function

    '2値、白黒
    Private Function GrayScale2Sirokuro(img As BitmapSource) As BitmapSource
        Dim wb As New WriteableBitmap(img)
        Dim h As Integer = wb.PixelHeight
        Dim w As Integer = wb.PixelWidth
        Dim stride As Integer = wb.BackBufferStride
        Dim pixels(h * stride - 1) As Byte
        Dim p As Long
        wb.CopyPixels(pixels, stride, 0)

        For y As Integer = 0 To h - 1
            For x As Integer = 0 To w - 1
                p = y * stride + (x * 4) '基準セルの場所
                If pixels(p) > 128 Then
                    Call SetColorGray(pixels, p, 255)
                Else
                    Call SetColorGray(pixels, p, 0)
                End If
            Next
        Next
        wb.WritePixels(New Int32Rect(0, 0, w, h), pixels, stride, 0)
        Return wb
    End Function

    'パターンディザ2x2カラー版
    Private Function Ditering2x2Color2(img As BitmapSource) As BitmapSource
        Dim thresholdMap As Single()() = New Single(1)() {
           New Single(1) {1.0F / 5.0F, 3.0F / 5.0F},
           New Single(1) {4.0F / 5.0F, 2.0F / 5.0F}
       }
        Dim wb As New WriteableBitmap(img)
        Dim h As Integer = wb.PixelHeight
        Dim w As Integer = wb.PixelWidth
        Dim stride As Integer = wb.BackBufferStride
        Dim pixels(h * stride - 1) As Byte
        Dim p As Long
        wb.CopyPixels(pixels, stride, 0)

        Dim threshold As Single
        For y As Integer = 0 To h - 1
            For x As Integer = 0 To w - 1
                p = y * stride + (x * 4) '基準セルの場所
                threshold = thresholdMap(x Mod 2)(y Mod 2)
                SetColor(threshold, pixels, p)
            Next
        Next
        wb.WritePixels(New Int32Rect(0, 0, w, h), pixels, stride, 0)
        Return wb
    End Function



    Private Sub SaveImage(bs As BitmapSource, SavePath As String)
        Dim enc As New BmpBitmapEncoder
        Dim bf As BitmapFrame = BitmapFrame.Create(bs)
        enc.Frames.Add(bf)
        Using fs As New System.IO.FileStream(SavePath, System.IO.FileMode.Create)
            enc.Save(fs)
        End Using
    End Sub

End Class
