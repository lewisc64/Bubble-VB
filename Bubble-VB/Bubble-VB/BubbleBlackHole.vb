Public Class BubbleBlackHole
    Inherits Bubble

    Public Sub New(x As Integer, y As Integer, angle As Integer, radius As Integer, availableColors As List(Of Integer))
        MyBase.New(x, y, angle, radius, availableColors)
        image = VBGame.Assets.images("bubble_black_hole")
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
                    Dim neighbors As List(Of Cell) = grid.getNeighbors(Cell.ix, Cell.iy)
                    For Each nCell As Cell In neighbors.ToList()
                        For Each nCell2 As Cell In grid.getNeighbors(nCell.ix, nCell.iy)
                            If Not neighbors.Contains(nCell2) AndAlso Cell.getIXIY() <> nCell2.getIXIY() Then
                                neighbors.Add(nCell2)
                            End If
                        Next
                    Next
                    For Each nCell As Cell In neighbors
                        nCell.pop(grid)
                    Next
                    VBGame.Assets.sounds("black_hole").play()
                    grid.update()
                    Return True
                End If
            End If
        Next
        Return False
    End Function

End Class
