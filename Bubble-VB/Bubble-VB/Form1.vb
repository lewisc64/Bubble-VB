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

        VBGame.Assets.sounds.Add("shoot", New VBGame.Sound("assets/sounds/shoot.mp3"))
        VBGame.Assets.sounds.Add("pop", New VBGame.Sound("assets/sounds/pop.mp3"))
        VBGame.Assets.sounds.Add("bounce", New VBGame.Sound("assets/sounds/bounce.mp3"))
        VBGame.Assets.sounds.Add("snap", New VBGame.Sound("assets/sounds/snap.mp3"))
        VBGame.Assets.sounds.Add("music", New VBGame.Sound("assets/sounds/music/Marty_Gots_a_Plan.mp3"))

        VBGame.Assets.images.Add("bg", VBGame.Images.load("assets/images/bg.png"))
        VBGame.Assets.images.Add("sidebar", VBGame.Images.load("assets/images/sidebar.png"))
        display = New VBGame.Display(Me, New Size(800, 600), "Bubble-VB")
        thread.Start()
    End Sub

    Public Sub gameloop()

        VBGame.Assets.sounds("music").play(True)

        Dim radius As Integer = 15

        Dim grid As Grid = New Grid(New Size(display.width / 40 - 1, 22), radius, 6, 21)

        Dim background As New VBGame.Surface(display.getRect(), display)
        background.blit(VBGame.Assets.images("bg"), New Point(0, 0))
        background.blit(VBGame.Assets.images("sidebar"), New Point(585, 0))

        Dim frames As Integer = 0

        Dim player As New Player(New Size(585, display.height), radius)

        grid.calculateExposed()

        Dim bubbles As New List(Of Bubble)

        While True
            frames += 1

            background.update()

            If Not player.ready AndAlso bubbles.Count = 0 Then
                player.ready = True
            End If

            For Each e As VBGame.MouseEvent In display.getMouseEvents()
                player.handleMouseControls(e, bubbles)
            Next

            grid.draw(display)

            player.draw(display)

            For Each Bubble As Bubble In bubbles.ToList()
                Bubble.draw(display)
                If Bubble.handle(grid) Then
                    bubbles.Remove(Bubble)
                End If
            Next

            'For Each Cell As Cell In grid.exposed
            '    display.drawCircle(New VBGame.Circle(Cell.Bubble.getCenter, Cell.Bubble.realRadius), Color.White)
            'Next

            'For Each Cell As Cell In grid.getUnoccupied()
            '    display.drawRect(New Rectangle(Cell.x, Cell.y, 30, 30), Color.Black)
            'Next

            display.drawText(New Point(0, 0), display.getTime(), VBGame.Colors.green)
            display.update()
            display.clockTick(60)

        End While

    End Sub

End Class
