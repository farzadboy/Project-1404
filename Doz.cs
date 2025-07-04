using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication10
{
    class Program
    {
        static void Main(string[] args)
        {
            char[,] Board = new char[3, 3] {{ '?' , '?' , '?'},
                                           { '?' , '?' , '?'},
                                           { '?' , '?' , '?'}};

            bool END = false;
            int turn = 1;

            while (END = true)
            {

                for (int i = 0; i < Board.GetLength(0); i++)
                {
                    for (int j = 0; j < Board.GetLength(1); j++)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;

                        Console.Write(Board[i, j]);
                    }
                    Console.WriteLine();
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("the turn :" + turn);

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("enter ROw [?,0]:");
                int row = int.Parse(Console.ReadLine());

                Console.WriteLine("enter cloum [?,0]:");
                int colum = int.Parse(Console.ReadLine());
                row--;
                colum--;



                if (turn == 1)
                {
                    if (Board[row, colum] == 'O')
                    {
                        Console.ForegroundColor = ConsoleColor.Red;

                        Console.WriteLine("please try again");
                    }
                    else
                    {

                        Board[row, colum] = 'X';
                        turn *= -1;
                    }
                }
                else if (turn == -1)
                {
                    if (Board[row, colum] == 'X')
                    {
                        Console.ForegroundColor = ConsoleColor.Red;

                        Console.WriteLine("please try again");
                    }
                    else
                    {


                        Board[row, colum] = 'O';
                        turn *= -1;

                    }
                }
                if (CheckWin(Board))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    if (turn == 1)
                    {
                        Console.WriteLine("win :" + 'o');
                       
                    }
                    else
                    {
                        Console.WriteLine("win :" + 'x');

                    }
                    
                    Console.ReadKey();
                }


            }
        }
        static bool CheckWin(char[,] board)
        {
            
            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0] != '?' && board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2])
                    return true;
                if (board[0, i] != '?' && board[0, i] == board[1, i] && board[1, i] == board[2, i])
                    return true;
            }

            
            if (board[0, 0] != '?' && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
                return true;
            if (board[0, 2] != '?' && board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])
                return true;

            return false;
        }

    }
}
