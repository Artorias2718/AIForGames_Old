using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AISandbox
{
    public class MarkGrid : MonoBehaviour
    {
        #region Variables
        public Mode mode;

        private Grid _grid;
        private float _node_width;
        private float _node_height;
        private int[,] _state;
        private int _depth;
        private List<int> _win_states;
        private int _turn;
        private List<GridNode> _available_nodes;
        #endregion

        #region Getters_Setters

        #endregion

        #region Unity
        private void Start()
        {
            _grid = GetComponent<Grid>();

            switch (mode)
            {
                case Mode.THREE_BY_THREE:
                    _grid.m_rows = 3;
                    _grid.m_columns = 3;
                    _depth = 2;

                    _win_states = new List<int>();
                    for (int i = 0; i < 8; ++i)
                    {
                        _win_states.Add(i);
                    }
                    break;
                case Mode.SIX_BY_SIX:
                    _grid.m_rows = 6;
                    _grid.m_columns = 6;
                    _depth = 5;

                    _win_states = new List<int>();

                    for (int i = 0; i < 54; ++i)
                    {
                        _win_states.Add(i);
                    }
                    break;
            }

            // Create and center the grid
            _grid.Create(_grid.m_rows, _grid.m_columns);
            Vector2 grid_size = _grid.Size;
            Vector2 grid_position = 0.5f * new Vector2(-grid_size.x, grid_size.y);
            transform.position = grid_position;

            _state = new int[_grid.m_rows, _grid.m_columns];

            _node_width = GetComponentInChildren<GridNode>().GetComponent<Renderer>().bounds.size.x;
            _node_height = GetComponentInChildren<GridNode>().GetComponent<Renderer>().bounds.size.y;

            for (int y = 0; y < _grid.m_columns; ++y)
            {
                for (int x = 0; x < _grid.m_rows; ++x)
                {
                    _state[x, y] = 0;
                }
            }
        }

        private void Update()
        {
            if (_turn == 0)
            {
                HumanMove();
            }
            else
            {
                AIMove();
            }
        }
        #endregion

        #region Custom 
        private void HumanMove()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (_grid.GetNodeFromPosition(mouse_position))
                {
                    GridNode node = _grid.GetNodeFromPosition(mouse_position);

                    if (node.Value == 0)
                    {
                        node.Value = 1;
                        _state[node.m_row, node.m_column] = 1;
                        _turn = 1;
                    }
                }
            }
        }

        private void AIMove()
        {
            _available_nodes = _grid.Nodes.Cast<GridNode>().Where(item => item.Value == 0).ToList();
            AIMove optimal_move = MiniMax(_state, _depth, 2);
            _grid.Nodes[optimal_move.y, optimal_move.x].Value = 2;
            _state[optimal_move.y, optimal_move.x] = 2;
            _turn = 0;
        }

        // NegaMax: Rather than maximizing the score for one player,
        // and minimizing the score for the other, simply
        // minimize your opponent's score, negate it, and that
        // becomes your maximum score 

        private AIMove MiniMax(int[,] i_board, int i_depth, int i_player)
        {
            int opponent = (i_player == 1) ? 2 : 1;

            // Base case
            if (CheckState(i_board) == i_player)
            {
                return new AIMove(10 - i_depth);
            }
            else if (CheckState(i_board) == opponent)
            {
                return new AIMove(-10 - i_depth);
            }
            else if (IsFull(i_board))
            {
                return new AIMove(-1);
            }

            List<AIMove> moves = new List<AIMove>();

            // Recursive search
            for (int y = 0; y < i_board.GetLength(0); ++y)
            {
                for (int x = 0; x < i_board.GetLength(1); ++x)
                {
                    if (i_board[y, x] == 0)
                    {
                        AIMove move = new AIMove();
                        move.x = x;
                        move.y = y;

                        i_board[y, x] = i_player;
                        move.score = MiniMax(i_board, i_depth - 1, opponent).score;
                        moves.Add(move);

                        i_board[y, x] = 0;
                    }
                }
            }

            int best_move = 0;

            // Find best move for current player
            if(i_player == 2)
            {
                float best_score = -Mathf.Infinity;
                for(int i = 0; i < moves.Count; ++i)
                {
                    if(moves[i].score > best_score)
                    {
                        best_move = i;
                        best_score = moves[i].score;
                    }
                }
            }
            else
            {
                float best_score = Mathf.Infinity;
                for (int i = 0; i < moves.Count; ++i)
                {
                    if (moves[i].score < best_score)
                    {
                        best_move = i;
                        best_score = moves[i].score;
                    }
                }
            }

            return moves[best_move];
        }

        // Note: When Using NegaMax, you analyze everything from the perspective
        // of your opponent (you no longer need to switch between you and your
        // opponent playing). Hence, calculate the score as if you were your opponent.

        //private int Score(int[,] i_board, int i_depth, int i_player)
        //{

        //}

        private int CheckState(int[,] i_board)
        {
            if (mode == Mode.THREE_BY_THREE)
            {
                #region rows
                #region row 0
                if (i_board[0, 0] != 0)
                {
                    if ((i_board[0, 0] == i_board[0, 1]) && (i_board[0, 1] == i_board[0, 2]) && (i_board[0, 2] == i_board[0, 0]))
                    {
                        return i_board[0, 0];
                    }
                }
                #endregion
                #region row 1
                if (i_board[1, 0] != 0)
                {
                    if ((i_board[1, 0] == i_board[1, 1]) && (i_board[1, 1] == i_board[1, 2]) && (i_board[1, 2] == i_board[1, 0]))
                    {
                        return i_board[1, 0];
                    }
                }
                #endregion
                #region row 2
                if (i_board[2, 0] != 0)
                {
                    if ((i_board[2, 0] == i_board[2, 1]) && (i_board[2, 1] == i_board[2, 2]) && (i_board[2, 2] == i_board[2, 0]))
                    {
                        return i_board[2, 0];
                    }
                }
                #endregion
                #endregion
                #region columns
                #region column 0
                if (i_board[0, 0] != 0)
                {
                    if ((i_board[0, 0] == i_board[1, 0]) && (i_board[1, 0] == i_board[2, 0]) && (i_board[2, 0] == i_board[0, 0]))
                    {
                        return i_board[0, 0];
                    }
                }
                #endregion
                #region column 1
                if (i_board[0, 1] != 0)
                {
                    if ((i_board[0, 1] == i_board[1, 1]) && (i_board[1, 1] == i_board[2, 1]) && (i_board[2, 1] == i_board[0, 1]))
                    {
                        return i_board[0, 1];
                    }
                }
                #endregion
                #region column 2
                if (i_board[0, 2] != 0)
                {
                    if ((i_board[0, 2] == i_board[1, 2]) && (i_board[1, 2] == i_board[2, 2]) && (i_board[2, 2] == i_board[0, 2]))
                    {
                        return i_board[0, 2];
                    }
                }
                #endregion
                #endregion
                #region diagonals
                #region diagonal 0
                if (i_board[0, 0] != 0)
                {
                    if ((i_board[0, 0] == i_board[1, 1]) && (i_board[1, 1] == i_board[2, 2]) && (i_board[2, 2] == i_board[0, 0]))
                    {
                        return i_board[0, 0];
                    }
                }
                #endregion
                #region diagonal 1
                if (i_board[0, 2] != 0)
                {
                    if ((i_board[0, 2] == i_board[1, 1]) && (i_board[1, 1] == i_board[2, 0]) && (i_board[2, 0] == i_board[0, 2]))
                    {
                        return i_board[0, 2];
                    }
                }
                #endregion
                #endregion
            }
            if (mode == Mode.SIX_BY_SIX)
            {
                #region rows
                #region row 0
                if (i_board[0, 0] != 0)
                {
                    if ((i_board[0, 0] == i_board[0, 1]) && (i_board[0, 1] == i_board[0, 2]) && (i_board[0, 2] == i_board[0, 3]) && (i_board[0, 3] == i_board[0, 0]))
                    {
                        if (_win_states.Contains(0))
                            return i_board[0, 0];
                    }
                }
                if (i_board[0, 1] != 0)
                {
                    if ((i_board[0, 1] == i_board[0, 2]) && (i_board[0, 2] == i_board[0, 3]) && (i_board[0, 3] == i_board[0, 4]) && (i_board[0, 4] == i_board[0, 1]))
                    {
                        if (_win_states.Contains(1))
                            return i_board[0, 1];
                    }
                }
                if (i_board[0, 2] != 0)
                {
                    if ((i_board[0, 2] == i_board[0, 3]) && (i_board[0, 3] == i_board[0, 4]) && (i_board[0, 4] == i_board[0, 5]) && (i_board[0, 5] == i_board[0, 2]))
                    {
                        if (_win_states.Contains(2))
                            return i_board[0, 2];
                    }
                }
                #endregion
                #region row 1
                if (i_board[1, 0] != 0)
                {
                    if ((i_board[1, 0] == i_board[1, 1]) && (i_board[1, 1] == i_board[1, 2]) && (i_board[1, 2] == i_board[1, 3]) && (i_board[1, 3] == i_board[1, 0]))
                    {
                        if (_win_states.Contains(3))
                            return i_board[1, 0];
                    }
                }
                if (i_board[1, 1] != 0)
                {
                    if ((i_board[1, 1] == i_board[1, 2]) && (i_board[1, 2] == i_board[1, 3]) && (i_board[1, 3] == i_board[1, 4]) && (i_board[1, 4] == i_board[1, 1]))
                    {
                        if (_win_states.Contains(4))
                            return i_board[1, 1];
                    }
                }
                if (i_board[1, 2] != 0)
                {
                    if ((i_board[1, 2] == i_board[1, 3]) && (i_board[1, 3] == i_board[1, 4]) && (i_board[1, 4] == i_board[1, 5]) && (i_board[1, 5] == i_board[1, 2]))
                    {
                        if (_win_states.Contains(5))
                            return i_board[1, 2];
                    }
                }
                #endregion
                #region row 2
                if (i_board[2, 0] != 0)
                {
                    if ((i_board[2, 0] == i_board[2, 1]) && (i_board[2, 1] == i_board[2, 2]) && (i_board[2, 2] == i_board[2, 3]) && (i_board[2, 3] == i_board[2, 0]))
                    {
                        if (_win_states.Contains(6))
                            return i_board[2, 0];
                    }
                }
                if (i_board[2, 1] != 0)
                {
                    if ((i_board[2, 1] == i_board[2, 2]) && (i_board[2, 2] == i_board[2, 3]) && (i_board[2, 3] == i_board[2, 4]) && (i_board[2, 4] == i_board[2, 1]))
                    {
                        if (_win_states.Contains(7))
                            return i_board[2, 1];
                    }
                }
                if (i_board[2, 2] != 0)
                {
                    if ((i_board[2, 2] == i_board[2, 3]) && (i_board[2, 3] == i_board[2, 4]) && (i_board[2, 4] == i_board[2, 5]) && (i_board[2, 5] == i_board[2, 2]))
                    {
                        if (_win_states.Contains(8))
                            return i_board[2, 2];
                    }
                }
                #endregion
                #region row 3
                if (i_board[3, 0] != 0)
                {
                    if ((i_board[3, 0] == i_board[3, 1]) && (i_board[3, 1] == i_board[3, 2]) && (i_board[3, 2] == i_board[3, 3]) && (i_board[3, 3] == i_board[3, 0]))
                    {
                        if (_win_states.Contains(9))
                            return i_board[3, 0];
                    }
                }
                if (i_board[3, 1] != 0)
                {
                    if ((i_board[3, 1] == i_board[3, 2]) && (i_board[3, 2] == i_board[3, 3]) && (i_board[3, 3] == i_board[3, 4]) && (i_board[3, 4] == i_board[3, 1]))
                    {
                        if (_win_states.Contains(10))
                            return i_board[3, 1];
                    }
                }
                if (i_board[3, 2] != 0)
                {
                    if ((i_board[3, 2] == i_board[3, 3]) && (i_board[3, 3] == i_board[3, 4]) && (i_board[3, 4] == i_board[3, 5]) && (i_board[3, 5] == i_board[3, 2]))
                    {
                        if (_win_states.Contains(11))
                            return i_board[3, 2];
                    }
                }
                #endregion
                #region row 4
                if (i_board[4, 0] != 0)
                {
                    if ((i_board[4, 0] == i_board[4, 1]) && (i_board[4, 1] == i_board[4, 2]) && (i_board[4, 2] == i_board[4, 3]) && (i_board[4, 3] == i_board[4, 0]))
                    {
                        if (_win_states.Contains(12))
                            return i_board[4, 0];
                    }
                }
                if (i_board[4, 1] != 0)
                {
                    if ((i_board[4, 1] == i_board[4, 2]) && (i_board[4, 2] == i_board[4, 3]) && (i_board[4, 3] == i_board[4, 4]) && (i_board[4, 4] == i_board[4, 1]))
                    {
                        if (_win_states.Contains(13))
                            return i_board[4, 1];
                    }
                }
                if (i_board[4, 2] != 0)
                {
                    if ((i_board[4, 2] == i_board[4, 3]) && (i_board[4, 3] == i_board[4, 4]) && (i_board[4, 4] == i_board[4, 5]) && (i_board[4, 5] == i_board[4, 2]))
                    {
                        if (_win_states.Contains(14))
                            return i_board[4, 2];
                    }
                }
                #endregion
                #region row 5
                if (i_board[5, 0] != 0)
                {
                    if ((i_board[5, 0] == i_board[5, 1]) && (i_board[5, 1] == i_board[5, 2]) && (i_board[5, 2] == i_board[5, 3]) && (i_board[5, 3] == i_board[5, 0]))
                    {
                        if (_win_states.Contains(15))
                            return i_board[5, 0];
                    }
                }
                if (i_board[5, 1] != 0)
                {
                    if ((i_board[5, 1] == i_board[5, 2]) && (i_board[5, 2] == i_board[5, 3]) && (i_board[5, 3] == i_board[5, 4]) && (i_board[5, 4] == i_board[5, 1]))
                    {
                        if (_win_states.Contains(16))
                            return i_board[5, 1];
                    }
                }
                if (i_board[3, 2] != 0)
                {
                    if ((i_board[5, 2] == i_board[5, 3]) && (i_board[5, 3] == i_board[5, 4]) && (i_board[5, 4] == i_board[5, 5]) && (i_board[5, 5] == i_board[5, 2]))
                    {
                        if (_win_states.Contains(17))
                            return i_board[5, 2];
                    }
                }
                #endregion
                #endregion
                #region columns
                #region column 0
                if (i_board[0, 0] != 0)
                {
                    if ((i_board[0, 0] == i_board[1, 0]) && (i_board[1, 0] == i_board[2, 0]) && (i_board[2, 0] == i_board[3, 0]) && (i_board[3, 0] == i_board[0, 0]))
                    {
                        if (_win_states.Contains(18))
                            return i_board[0, 0];
                    }
                }
                if (i_board[1, 0] != 0)
                {
                    if ((i_board[1, 0] == i_board[2, 0]) && (i_board[2, 0] == i_board[3, 0]) && (i_board[3, 0] == i_board[4, 0]) && (i_board[4, 0] == i_board[1, 0]))
                    {
                        if (_win_states.Contains(19))
                            return i_board[1, 0];
                    }
                }
                if (i_board[2, 0] != 0)
                {
                    if ((i_board[2, 0] == i_board[3, 0]) && (i_board[3, 0] == i_board[4, 0]) && (i_board[4, 0] == i_board[5, 0]) && (i_board[5, 0] == i_board[2, 0]))
                    {
                        if (_win_states.Contains(20))
                            return i_board[2, 0];
                    }
                }
                #endregion
                #region column 1
                if (i_board[0, 1] != 0)
                {
                    if ((i_board[0, 1] == i_board[1, 1]) && (i_board[1, 1] == i_board[2, 1]) && (i_board[2, 1] == i_board[3, 1]) && (i_board[3, 1] == i_board[0, 1]))
                    {
                        if (_win_states.Contains(21))
                            return i_board[0, 1];
                    }
                }
                if (i_board[1, 1] != 0)
                {
                    if ((i_board[1, 1] == i_board[2, 1]) && (i_board[2, 1] == i_board[3, 1]) && (i_board[3, 1] == i_board[4, 1]) && (i_board[4, 1] == i_board[1, 1]))
                    {
                        if (_win_states.Contains(22))
                            return i_board[1, 1];
                    }
                }
                if (i_board[2, 1] != 0)
                {
                    if ((i_board[2, 1] == i_board[3, 1]) && (i_board[3, 1] == i_board[4, 1]) && (i_board[4, 1] == i_board[5, 1]) && (i_board[5, 1] == i_board[2, 1]))
                    {
                        if (_win_states.Contains(23))
                            return i_board[2, 1];
                    }
                }
                #endregion
                #region column 2
                if (i_board[0, 2] != 0)
                {
                    if ((i_board[0, 2] == i_board[1, 2]) && (i_board[1, 2] == i_board[2, 2]) && (i_board[2, 2] == i_board[3, 2]) && (i_board[3, 2] == i_board[0, 2]))
                    {
                        if (_win_states.Contains(24))
                            return i_board[0, 2];
                    }
                }
                if (i_board[1, 2] != 0)
                {
                    if ((i_board[1, 2] == i_board[2, 2]) && (i_board[2, 2] == i_board[3, 2]) && (i_board[3, 2] == i_board[4, 2]) && (i_board[4, 2] == i_board[1, 2]))
                    {
                        if (_win_states.Contains(25))
                            return i_board[1, 2];
                    }
                }
                if (i_board[2, 2] != 0)
                {
                    if ((i_board[2, 2] == i_board[3, 2]) && (i_board[3, 2] == i_board[4, 2]) && (i_board[4, 2] == i_board[5, 2]) && (i_board[5, 2] == i_board[2, 2]))
                    {
                        if (_win_states.Contains(26))
                            return i_board[2, 2];
                    }
                }
                #endregion
                #region column 3
                if (i_board[0, 3] != 0)
                {
                    if ((i_board[0, 3] == i_board[1, 3]) && (i_board[1, 3] == i_board[2, 3]) && (i_board[2, 3] == i_board[3, 3]) && (i_board[3, 3] == i_board[0, 3]))
                    {
                        if (_win_states.Contains(27))
                            return i_board[0, 3];
                    }
                }
                if (i_board[1, 3] != 0)
                {
                    if ((i_board[1, 3] == i_board[2, 3]) && (i_board[2, 3] == i_board[3, 3]) && (i_board[3, 3] == i_board[4, 3]) && (i_board[4, 3] == i_board[1, 3]))
                    {
                        if (_win_states.Contains(28))
                            return i_board[1, 3];
                    }
                }
                if (i_board[2, 3] != 0)
                {
                    if ((i_board[2, 3] == i_board[3, 3]) && (i_board[3, 3] == i_board[4, 3]) && (i_board[4, 3] == i_board[5, 3]) && (i_board[5, 3] == i_board[2, 3]))
                    {
                        if (_win_states.Contains(29))
                            return i_board[2, 3];
                    }
                }
                #endregion
                #region column 4
                if (i_board[0, 4] != 0)
                {
                    if ((i_board[0, 4] == i_board[1, 4]) && (i_board[1, 4] == i_board[2, 4]) && (i_board[2, 4] == i_board[3, 4]) && (i_board[3, 4] == i_board[0, 4]))
                    {
                        if (_win_states.Contains(30))
                            return i_board[0, 4];
                    }
                }
                if (i_board[1, 4] != 0)
                {
                    if ((i_board[1, 4] == i_board[2, 4]) && (i_board[2, 4] == i_board[3, 4]) && (i_board[3, 4] == i_board[4, 4]) && (i_board[4, 4] == i_board[1, 4]))
                    {
                        if (_win_states.Contains(31))
                            return i_board[1, 4];
                    }
                }
                if (i_board[2, 4] != 0)
                {
                    if ((i_board[2, 4] == i_board[3, 4]) && (i_board[3, 4] == i_board[4, 4]) && (i_board[4, 4] == i_board[5, 4]) && (i_board[5, 4] == i_board[2, 4]))
                    {
                        if (_win_states.Contains(32))
                            return i_board[2, 4];
                    }
                }
                #endregion
                #region column 5
                if (i_board[0, 5] != 0)
                {
                    if ((i_board[0, 5] == i_board[1, 5]) && (i_board[1, 5] == i_board[2, 5]) && (i_board[2, 5] == i_board[3, 5]) && (i_board[3, 5] == i_board[0, 5]))
                    {
                        if (_win_states.Contains(33))
                            return i_board[0, 5];
                    }
                }
                if (i_board[1, 5] != 0)
                {
                    if ((i_board[1, 5] == i_board[2, 5]) && (i_board[2, 5] == i_board[3, 5]) && (i_board[3, 5] == i_board[4, 5]) && (i_board[4, 5] == i_board[1, 5]))
                    {
                        if (_win_states.Contains(34))
                            return i_board[1, 5];
                    }
                }
                if (i_board[2, 5] != 0)
                {
                    if ((i_board[2, 5] == i_board[3, 5]) && (i_board[3, 5] == i_board[4, 5]) && (i_board[4, 5] == i_board[5, 5]) && (i_board[5, 5] == i_board[2, 5]))
                    {
                        if (_win_states.Contains(35))
                            return i_board[2, 5];
                    }
                }
                #endregion
                #endregion
                #region diagonals
                #region diagonal 0
                if (i_board[2, 0] != 0)
                {
                    if ((i_board[2, 0] == i_board[3, 1]) && (i_board[3, 1] == i_board[4, 2]) && (i_board[4, 2] == i_board[5, 3]) && (i_board[5, 3] == i_board[2, 0]))
                    {
                        if (_win_states.Contains(36))
                            return i_board[2, 0];
                    }
                }
                #endregion
                #region diagonal 1
                if (i_board[1, 0] != 0)
                {
                    if ((i_board[1, 0] == i_board[2, 1]) && (i_board[2, 1] == i_board[3, 2]) && (i_board[3, 2] == i_board[4, 3]) && (i_board[4, 3] == i_board[1, 0]))
                    {
                        if (_win_states.Contains(37))
                            return i_board[1, 0];
                    }
                }
                if (i_board[2, 1] != 0)
                {
                    if ((i_board[2, 1] == i_board[3, 2]) && (i_board[3, 2] == i_board[4, 3]) && (i_board[4, 3] == i_board[5, 4]) && (i_board[5, 4] == i_board[2, 1]))
                    {
                        if (_win_states.Contains(38))
                            return i_board[2, 1];
                    }
                }
                #endregion
                #region diagonal 2
                if (i_board[0, 0] != 0)
                {
                    if ((i_board[0, 0] == i_board[1, 1]) && (i_board[1, 1] == i_board[2, 2]) && (i_board[2, 2] == i_board[3, 3]) && (i_board[3, 3] == i_board[0, 0]))
                    {
                        if (_win_states.Contains(39))
                            return i_board[0, 0];
                    }
                }
                if (i_board[1, 1] != 0)
                {
                    if ((i_board[1, 1] == i_board[2, 2]) && (i_board[2, 2] == i_board[3, 3]) && (i_board[3, 3] == i_board[4, 4]) && (i_board[4, 4] == i_board[1, 1]))
                    {
                        if (_win_states.Contains(40))
                            return i_board[1, 1];
                    }
                }
                if (i_board[2, 2] != 0)
                {
                    if ((i_board[2, 2] == i_board[3, 3]) && (i_board[3, 3] == i_board[4, 4]) && (i_board[4, 4] == i_board[5, 5]) && (i_board[5, 5] == i_board[2, 2]))
                    {
                        if (_win_states.Contains(41))
                            return i_board[2, 2];
                    }
                }
                #endregion
                #region diagonal 3
                if (i_board[0, 1] != 0)
                {
                    if ((i_board[0, 1] == i_board[1, 2]) && (i_board[1, 2] == i_board[2, 3]) && (i_board[2, 3] == i_board[3, 4]) && (i_board[3, 4] == i_board[0, 1]))
                    {
                        if (_win_states.Contains(42))
                            return i_board[0, 1];
                    }
                }
                if (i_board[1, 2] != 0)
                {
                    if ((i_board[1, 2] == i_board[2, 3]) && (i_board[2, 3] == i_board[3, 4]) && (i_board[3, 4] == i_board[4, 5]) && (i_board[4, 5] == i_board[1, 2]))
                    {
                        if (_win_states.Contains(43))
                            return i_board[1, 2];
                    }
                }
                #endregion
                #region diagonal 4
                if (i_board[0, 2] != 0)
                {
                    if ((i_board[0, 2] == i_board[1, 3]) && (i_board[1, 3] == i_board[2, 4]) && (i_board[2, 4] == i_board[3, 5]) && (i_board[3, 5] == i_board[0, 2]))
                    {
                        if (_win_states.Contains(44))
                            return i_board[0, 2];
                    }
                }
                #endregion

                #region diagonal 5
                if (i_board[2, 5] != 0)
                {
                    if ((i_board[2, 5] == i_board[3, 4]) && (i_board[3, 4] == i_board[4, 3]) && (i_board[4, 3] == i_board[5, 2]) && (i_board[5, 2] == i_board[2, 5]))
                    {
                        if (_win_states.Contains(45))
                            return i_board[2, 5];
                    }
                }
                #endregion
                #region diagonal 6
                if (i_board[1, 5] != 0)
                {
                    if ((i_board[1, 5] == i_board[2, 4]) && (i_board[2, 4] == i_board[3, 3]) && (i_board[3, 3] == i_board[4, 2]) && (i_board[4, 2] == i_board[1, 5]))
                    {
                        if (_win_states.Contains(46))
                            return i_board[1, 5];
                    }
                }
                if (i_board[2, 4] != 0)
                {
                    if ((i_board[2, 4] == i_board[3, 3]) && (i_board[3, 3] == i_board[4, 2]) && (i_board[4, 2] == i_board[5, 1]) && (i_board[5, 1] == i_board[2, 4]))
                    {
                        if (_win_states.Contains(47))
                            return i_board[2, 4];
                    }
                }
                #endregion
                #region diagonal 7
                if (i_board[0, 5] != 0)
                {
                    if ((i_board[0, 5] == i_board[1, 4]) && (i_board[1, 4] == i_board[2, 3]) && (i_board[2, 3] == i_board[3, 2]) && (i_board[3, 2] == i_board[0, 5]))
                    {
                        if (_win_states.Contains(48))
                            return i_board[0, 5];
                    }
                }
                if (i_board[1, 4] != 0)
                {
                    if ((i_board[1, 4] == i_board[2, 3]) && (i_board[2, 3] == i_board[3, 2]) && (i_board[3, 2] == i_board[4, 1]) && (i_board[4, 1] == i_board[1, 4]))
                    {
                        if (_win_states.Contains(49))
                            return i_board[1, 4];
                    }
                }
                if (i_board[2, 3] != 0)
                {
                    if ((i_board[2, 3] == i_board[3, 2]) && (i_board[3, 2] == i_board[4, 1]) && (i_board[4, 1] == i_board[5, 0]) && (i_board[5, 0] == i_board[2, 3]))
                    {
                        if (_win_states.Contains(50))
                            return i_board[2, 3];
                    }
                }
                #endregion
                #region diagonal 8
                if (i_board[0, 4] != 0)
                {
                    if ((i_board[0, 4] == i_board[1, 3]) && (i_board[1, 3] == i_board[2, 2]) && (i_board[2, 2] == i_board[3, 1]) && (i_board[3, 1] == i_board[0, 4]))
                    {
                        if (_win_states.Contains(51))
                            return i_board[0, 4];
                    }
                }
                if (i_board[1, 3] != 0)
                {
                    if ((i_board[1, 3] == i_board[2, 2]) && (i_board[2, 2] == i_board[3, 1]) && (i_board[3, 1] == i_board[4, 0]) && (i_board[4, 0] == i_board[1, 3]))
                    {
                        if (_win_states.Contains(52))
                            return i_board[1, 3];
                    }
                }
                #endregion
                #region diagonal 9
                if (i_board[0, 3] != 0)
                {
                    if ((i_board[0, 3] == i_board[1, 2]) && (i_board[1, 2] == i_board[2, 1]) && (i_board[2, 1] == i_board[3, 0]) && (i_board[3, 0] == i_board[0, 3]))
                    {
                        if (_win_states.Contains(53))
                            return i_board[0, 3];
                    }
                }
                #endregion
                #endregion
            }

            return 0;
        }

        private bool IsFull(int[,] i_board)
        {
            for (int y = 0; y < i_board.GetLength(0); ++y)
            {
                for (int x = 0; x < i_board.GetLength(1); ++x)
                {
                    if (i_board[y, x] == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion
    }
}