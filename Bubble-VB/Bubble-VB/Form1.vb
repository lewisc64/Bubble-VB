Imports System.Threading
Imports System.IO

Public Class Form1

    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg <> &HF Then
            MyBase.WndProc(m)
        End If
    End Sub

    Public display As VBGame.Display
    Public thread As New Thread(AddressOf mainloop)
    Public settings As New Settings

    Public Sub loadImage(key As String, relpath As String)
        Try
            VBGame.Assets.images.Add(key, VBGame.Images.load("assets/images/" & settings.theme & "/" & relpath))
        Catch ex As Exception
            VBGame.Assets.images.Add(key, VBGame.Images.load("assets/images/standard/" & relpath))
        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        settings = New Settings()
        If Not File.Exists("Settings.xml") Then
            settings.Save()
        Else
            VBGame.XMLIO.Read("Settings.xml", settings)
        End If

        Dim month As Integer = Today.Month
        If month = 12 Then
            settings.theme = "christmas"
        Else
            settings.theme = "standard"
        End If

        Try
            Bubble.images.Add(VBGame.Images.load("assets/images/" & settings.theme & "/bubbles/red.png"))
            Bubble.images.Add(VBGame.Images.load("assets/images/" & settings.theme & "/bubbles/yellow.png"))
            Bubble.images.Add(VBGame.Images.load("assets/images/" & settings.theme & "/bubbles/purple.png"))
            Bubble.images.Add(VBGame.Images.load("assets/images/" & settings.theme & "/bubbles/blue.png"))
            Bubble.images.Add(VBGame.Images.load("assets/images/" & settings.theme & "/bubbles/green.png"))
            Bubble.images.Add(VBGame.Images.load("assets/images/" & settings.theme & "/bubbles/cyan.png"))
        Catch
            MsgBox("Can't find bubble textures in current theme.")
            End
        End Try

        VBGame.Assets.sounds.Add("shoot", New VBGame.Sound("assets/sounds/shoot.mp3"))
        VBGame.Assets.sounds.Add("pop", New VBGame.Sound("assets/sounds/pop.mp3"))
        VBGame.Assets.sounds.Add("bounce", New VBGame.Sound("assets/sounds/bounce.mp3"))
        VBGame.Assets.sounds.Add("snap", New VBGame.Sound("assets/sounds/snap.mp3"))
        VBGame.Assets.sounds.Add("advance", New VBGame.Sound("assets/sounds/advance.mp3"))
        VBGame.Assets.sounds.Add("music", New VBGame.Sound("assets/sounds/music/Marty_Gots_a_Plan.mp3"))
        VBGame.Assets.sounds.Add("victory", New VBGame.Sound("assets/sounds/music/victory.mp3"))
        VBGame.Assets.sounds.Add("special_bounce", New VBGame.Sound("assets/sounds/special_bounce.mp3"))
        VBGame.Assets.sounds.Add("black_hole", New VBGame.Sound("assets/sounds/black_hole.mp3"))
        VBGame.Assets.sounds.Add("lightning", New VBGame.Sound("assets/sounds/lightning.mp3"))

        loadImage("bg", "bg.png")
        loadImage("sidebar", "sidebar.png")
        loadImage("star", "star.png")
        loadImage("bg_gold", "bg_gold.png")
        loadImage("bubble_black_hole", "bubbles/black_hole.png")
        loadImage("bubble_lightning", "bubbles/lightning.png")

        Player.specialBubbles.Add(GetType(BubbleBlackHole))

        Me.Icon = New Icon("assets/images/icon.ico")

        display = New VBGame.Display(Me, New Size(800, 600), "Bubble-VB")
        thread.Start()
    End Sub

    Public Sub mainloop()
        While True
            settings.handleScore(gameloop())
            display.getMouseEvents()
        End While
    End Sub

    Public Sub drawScore(grid As Grid)
        Dim color As Color
        If grid.won Then
            color = color.Gold
        Else
            color = VBGame.Colors.grey
        End If
        display.drawText(New Point(display.width - 205, 10), "Score: " & CStr(grid.score), color, New Font("Xpress Heavy SF", 16))
    End Sub

    Public Function getBackground(Optional won As Boolean = False) As VBGame.Surface

        Dim background As New VBGame.Surface(display.getRect(), display)

        background.blit(VBGame.Assets.images("bg"), New Point(0, 0))
        If won Then
            background.blit(VBGame.Assets.images("bg_gold"), New Point(0, 0))
        End If

        background.blit(VBGame.Assets.images("sidebar"), New Point(585, 0))

        Dim tx As Integer = display.width - 205
        Dim ty As Integer = 64

        Dim color As Color

        background.drawText(New Point(tx, ty), "Leaderboard:", VBGame.Colors.grey, New Font("Xpress Heavy SF", 16))
        ty += 16
        For Each Score As Score In settings.leaderboard
            ty += 16
            If Not IsNothing(Score) Then
                If Score.won Then
                    color = color.Gold
                Else
                    color = VBGame.Colors.grey
                End If
                background.drawText(New Point(tx, ty), Score.name, color, New Font("Xpress Heavy SF", 16))
                background.drawText(New Point(tx + 100, ty), Score.score, color, New Font("Xpress Heavy SF", 16))
            End If
        Next

        If settings.gamesWon > 0 Then
            background.blit(VBGame.Assets.images("star"), New Point(display.width - 160, 390))
            background.drawText(New Rectangle(display.width - 160, 390 + (VBGame.Assets.images("star").Height * (1 / 2)), VBGame.Assets.images("star").Width, VBGame.Assets.images("star").Height), settings.gamesWon, color.Gold, New Font("Xpress Heavy SF", 16))
        End If

        Return background

    End Function

    Public Function createGrid() As Grid
        Return New Grid(New Size(display.width / 40 - 1, 22), settings.radius, 0)
    End Function

    Public Function gameloop() As Grid

        VBGame.Assets.sounds("music").play(True)

        Dim grid As Grid = createGrid()

        Dim frames As Integer = 0

        Dim player As New Player(New Size(585, display.height), settings.radius)

        Dim background As VBGame.Surface = getBackground()

        grid.calculateExposed()

        Dim bubbles As New List(Of Bubble)

        Dim gridChanged As Boolean = False

        Dim startingRows As Integer = 11

        Dim rowsToAdd As Integer = startingRows

        Dim reset As New VBGame.Button(display, "Restart", New Rectangle(display.width - 205, 350, 195, 32))
        reset.fontname = "Xpress Heavy SF"
        reset.fontsize = 16
        reset.setColor(Color.FromArgb(0, 0, 0, 0), Color.FromArgb(100, 100, 100))
        reset.setTextColor(VBGame.Colors.grey, VBGame.Colors.grey)

        While True
            frames += 1

            background.update()

            If Not player.ready AndAlso bubbles.Count = 0 Then
                player.ready = True
            End If

            For Each e As VBGame.MouseEvent In display.getMouseEvents()

                If e.location.X < display.width - 215 Then
                    player.handleMouseControls(e, bubbles, grid.updateList)
                End If

                If reset.handle(e) Then
                    Return grid
                End If

                'If e.button = VBGame.MouseEvent.buttons.right And e.action = VBGame.MouseEvent.actions.up Then
                '    For Each Cell As Cell In grid.bubbles
                '        Cell.Bubble = Nothing
                '    Next
                '    grid.update()
                'End If

            Next

            For Each Cell As Cell In grid.updateList.ToList()
                If Not IsNothing(Cell.Bubble) Then
                    Cell.Bubble.draw(display)
                    Cell.Bubble.scale -= 0.1
                    If Cell.Bubble.scale <= 0.1 Then
                        grid.updateList.Remove(Cell)
                        Cell.Bubble = Nothing
                        gridChanged = True
                    End If
                Else
                    MsgBox("Empty cell in update list. This should never happen.")
                    grid.updateList.Remove(Cell)
                End If
            Next

            If gridChanged AndAlso grid.updateList.Count = 0 Then
                gridChanged = False
                grid.update()
            End If

            grid.draw(display)

            player.draw(display)

            drawScore(grid)

            For Each Bubble As Bubble In bubbles.ToList()
                Bubble.draw(display)
                If Bubble.handle(grid, player) Then
                    bubbles.Remove(Bubble)
                End If
            Next

            'For Each Cell As Cell In grid.exposed
            '    display.drawCircle(New VBGame.Circle(Cell.Bubble.getCenter, Cell.Bubble.realRadius - 5), Color.White)
            'Next

            'For Each Cell As Cell In grid.getUnoccupied()
            '    display.drawRect(New Rectangle(Cell.x, Cell.y, 30, 30), Color.Black)
            'Next

            If player.queue.Count = 0 AndAlso bubbles.Count = 0 Then
                rowsToAdd = 1
                player.populateQueue()
            End If

            If grid.lost Then
                Return grid
            End If

            If grid.exposed.Count = 0 AndAlso frames > 10 Then
                VBGame.Assets.sounds("victory").play()
                settings.gamesWon += 1
                settings.Save()
                background = getBackground(True)
                grid.won = True
                grid.score += 100000
                grid.addRow()
                rowsToAdd = startingRows - 1
            End If

            If rowsToAdd > 0 AndAlso frames Mod 2 = 0 Then
                For Each Cell As Cell In grid.updateList.ToList()
                    Cell.Bubble = Nothing
                    grid.updateList.Remove(Cell)
                Next
                grid.addRow()
                rowsToAdd -= 1
            End If

            reset.draw()

            'display.drawText(New Point(0, 0), display.getTime(), VBGame.Colors.green)
            display.update()
            display.clockTick(60)

        End While
        Return grid
    End Function

End Class
