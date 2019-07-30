namespace AISandbox
{
    public class AIMove
    {
        public int x;
        public int y;
        public int score;

        public AIMove() { }

        public AIMove(int i_score)
        {
            score = i_score;
        }
    }
}