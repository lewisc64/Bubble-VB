Imports System.Threading

Public Class Form1

    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg <> &HF Then
            MyBase.WndProc(m)
        End If
    End Sub

    Public display As VBGame.Display
    Public thread As New Thread(AddressOf gameloop)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Bubble.images.Add(VBGame.Images.load("assets/images/bubbles/red.png"))
        Bubble.images.Add(VBGame.Images.load("assets/images/bubbles/yellow.png"))
        Bubble.images.Add(VBGame.Images.load("assets/images/bubbles/purple.png"))
        Bubble.images.Add(VBGame.Images.load("assets/images/bubbles/blue.png"))
        Bubble.images.Add(VBGame.Images.load("assets/images/bubbles/green.png"))

        VBGame.Assets.images.Add("bg", VBGame.Images.load("assets/images/bg.png"))
        VBGame.Assets.images.Add("sidebar", VBGame.Images.load("assets/images/sidebar.png"))

        display = New VBGame.Display(Me, New Size(800, 600), "Bubble-VB")
        thread.Start()
    End Sub

    Public Sub gameloop()

        Dim grid As Grid = New Grid(display.width / 40 - 1, 22, 15, 22)

        Dim background As New VBGame.Surface(display.getRect(), display)
        background.blit(VBGame.Assets.images("bg"), New Point(0, 0))
        background.blit(VBGame.Assets.images("sidebar"), New Point(585, 0))

        grid.draw(background)

        Dim frames As Integer = 0

        While True

            frames += 1

            If frames Mod 60 = 0 Then
                background.blit(VBGame.Assets.images("bg"), New Point(0, 0))
                background.blit(VBGame.Assets.images("sidebar"), New Point(585, 0))
                grid.draw(background)
            End If

            background.update()

            display.drawText(New Point(0, 0), display.getTime(), VBGame.Colors.green)
            display.update()
            display.clockTick(60)

        End While

    End Sub

End Class
