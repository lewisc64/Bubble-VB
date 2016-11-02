Public Class BubbleLightning
    Inherits Bubble

    Public Sub New(x As Integer, y As Integer, angle As Integer, radius As Integer)
        MyBase.New(x, y, angle, radius)
        image = VBGame.Assets.images("bubble_lightning")
        scaledImage = VBGame.Images.resizeImage(image, scale, False)
    End Sub

    Public Overrides Function handle(ByRef grid As Grid, player As Player)
        move(True)
        If keepInBounds(New Rectangle(New Point(0, 0), grid.bounds), True, True) Then
            VBGame.Assets.sounds("special_bounce").play(False, True)
            If y + height < 0 Then
                Return True
            End If
        End If
        For Each Cell As Cell In grid.exposed
            If VBGame.Collisions.rect(getRect(), Cell.Bubble.getRect()) Then
                If VBGame.Collisions.circle(New VBGame.Circle(getCenter(), realRadius), New VBGame.Circle(Cell.Bubble.getCenter(), Cell.Bubble.realRadius)) Then
                    Cell.pop(grid)
                    For Each nCell As Cell In grid.getAbove(Cell.ix, Cell.iy)
                        nCell.pop(grid)
                    Next
                    VBGame.Assets.sounds("lightning").play()
                    grid.update()
                    Return True
                End If
            End If
        Next
        Return False
    End Function

End Class
