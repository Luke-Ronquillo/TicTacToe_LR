using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerOption
{
    NONE, //0
    X, // 1
    O // 2
}

public class TTT : MonoBehaviour
{
    public int Rows;
    public int Columns;
    [SerializeField] BoardView board;

    PlayerOption currentPlayer = PlayerOption.X;
    Cell[,] cells;

    // Start is called before the first frame update
    void Start()
    {
        cells = new Cell[Columns, Rows];

        board.InitializeBoard(Columns, Rows);

        for(int i = 0; i < Rows; i++)
        {
            for(int j = 0; j < Columns; j++)
            {
                cells[j, i] = new Cell();
                cells[j, i].current = PlayerOption.NONE;
            }
        }
    }

    public void MakeOptimalMove()
    {
        // First loop checks for winning moves. Second loop checks the other players winning moves and places at those spots.
        EndTurn();
        PlayerOption currentOp = currentPlayer;
        EndTurn();
        List<int> corners = new List<int>();
        int rowIndex = -1, colIndex = -1;
        int emptySpace = 0;
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                if (cells[i, j].current == PlayerOption.NONE)
                {
                    emptySpace++;
                    cells[i, j].current = currentPlayer;
                    if (GetWinner() == currentPlayer)
                    {
                        cells[i, j].current = PlayerOption.NONE;
                        ChooseSpace(i, j);
                        return;
                    }
                    cells[i, j].current = currentOp;
                    if (GetWinner() == currentOp)
                    {
                        rowIndex = i;
                        colIndex = j;
                    }
                    cells[i, j].current = PlayerOption.NONE;
                }
                if ((i == 0 && j == 0) || (i == 0 && j == 2) || (i == 2 && j == 0) || (i == 2 && j == 2))
                {
                    if (cells[i, j].current == currentPlayer)
                        corners.Add(1);
                    else if (cells[i, j].current == currentOp)
                        corners.Add(-1);
                    else
                        corners.Add(0);
                }
            }
        }
        if (rowIndex >= 0 && colIndex >= 0)
        { 
            ChooseSpace(rowIndex, colIndex);
            return;
        }
        else if (emptySpace == 9)
        { 
            ChooseSpace(0, 0);
            return;
        }
        else if (corners.Contains(-1) && cells[1, 1].current == PlayerOption.NONE)
        { 
            ChooseSpace(1, 1);
            return;
        }
        else if (cells[1, 1].current != PlayerOption.NONE)
        {
            while (corners.Contains(1))
            {
                int cornerIndex = corners.IndexOf(1);
                switch (cornerIndex)
                {
                    case 0:
                        if (cells[0, 1].current == PlayerOption.NONE)
                        { 
                            ChooseSpace(0, 1);
                            return;
                        }
                        if (cells[1, 0].current == PlayerOption.NONE)
                        {                             
                            ChooseSpace(1, 0);
                            return;
                        }
                        break;
                    case 1:
                        if (cells[0, 1].current == PlayerOption.NONE)
                        {
                            ChooseSpace(0, 1);
                            return;
                        }
                        if (cells[1, 2].current == PlayerOption.NONE)
                        {
                            ChooseSpace(1, 2);
                            return;
                        }
                        break;
                    case 2:
                        if (cells[2, 1].current == PlayerOption.NONE)
                        {
                            ChooseSpace(2, 1);
                            return;
                        }
                        if (cells[1, 0].current == PlayerOption.NONE)
                        {
                            ChooseSpace(1, 0);
                            return;
                        }
                        break;
                    case 3:
                        if (cells[1, 2].current == PlayerOption.NONE)
                        {
                            ChooseSpace(1, 2);
                            return;
                        }
                        if (cells[2, 1].current == PlayerOption.NONE)
                        {
                            ChooseSpace(2, 1);
                            return;
                        }
                        break;
                }
                corners[cornerIndex] = 0;
            }
        }
        do
        {
            rowIndex = UnityEngine.Random.Range(0, Rows);
            colIndex = UnityEngine.Random.Range(0, Columns);
        } while (cells[rowIndex, colIndex].current != PlayerOption.NONE && emptySpace > 0);
        ChooseSpace(rowIndex, colIndex);
    }

    public void ChooseSpace(int column, int row)
    {
        // can't choose space if game is over
        if (GetWinner() != PlayerOption.NONE)
            return;

        // can't choose a space that's already taken
        if (cells[column, row].current != PlayerOption.NONE)
            return;

        // set the cell to the player's mark
        cells[column, row].current = currentPlayer;

        // update the visual to display X or O
        board.UpdateCellVisual(column, row, currentPlayer);

        // if there's no winner, keep playing, otherwise end the game
        if(GetWinner() == PlayerOption.NONE)
            EndTurn();
        else
        {
            Debug.Log("GAME OVER!");
        }
    }

    public void EndTurn()
    {
        // increment player, if it goes over player 2, loop back to player 1
        currentPlayer += 1;
        if ((int)currentPlayer > 2)
            currentPlayer = PlayerOption.X;
    }

    public PlayerOption GetWinner()
    {
        // sum each row/column based on what's in each cell X = 1, O = -1, blank = 0
        // we have a winner if the sum = 3 (X) or -3 (O)
        int sum = 0;

        // check rows
        for (int i = 0; i < Rows; i++)
        {
            sum = 0;
            for (int j = 0; j < Columns; j++)
            {
                var value = 0;
                if (cells[j, i].current == PlayerOption.X)
                    value = 1;
                else if (cells[j, i].current == PlayerOption.O)
                    value = -1;

                sum += value;
            }

            if (sum == 3)
                return PlayerOption.X;
            else if (sum == -3)
                return PlayerOption.O;

        }

        // check columns
        for (int j = 0; j < Columns; j++)
        {
            sum = 0;
            for (int i = 0; i < Rows; i++)
            {
                var value = 0;
                if (cells[j, i].current == PlayerOption.X)
                    value = 1;
                else if (cells[j, i].current == PlayerOption.O)
                    value = -1;

                sum += value;
            }

            if (sum == 3)
                return PlayerOption.X;
            else if (sum == -3)
                return PlayerOption.O;

        }

        // check diagonals
        // top left to bottom right
        sum = 0;
        for(int i = 0; i < Rows; i++)
        {
            int value = 0;
            if (cells[i, i].current == PlayerOption.X)
                value = 1;
            else if (cells[i, i].current == PlayerOption.O)
                value = -1;

            sum += value;
        }

        if (sum == 3)
            return PlayerOption.X;
        else if (sum == -3)
            return PlayerOption.O;

        // top right to bottom left
        sum = 0;
        for (int i = 0; i < Rows; i++)
        {
            int value = 0;

            if (cells[Columns - 1 - i, i].current == PlayerOption.X)
                value = 1;
            else if (cells[Columns - 1 - i, i].current == PlayerOption.O)
                value = -1;

            sum += value;
        }

        if (sum == 3)
            return PlayerOption.X;
        else if (sum == -3)
            return PlayerOption.O;

        return PlayerOption.NONE;
    }
}
