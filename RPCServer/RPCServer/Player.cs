namespace RPCServer
{
    public class Player
    {
        public string Id { get; set; } // 한번 정해지면 바뀌지 않으므로 init
        public float X { get; set; }
        public float Y { get; set; }

        public Player(string id, float initialX, float initialY)
        {
            Id = id;
            X = initialX;
            Y = initialY;
        }

        public void Move(float newX, float newY)
        {
            // 여기에 이동 가능 여부, 속도 제한 등 복잡한 로직이 들어갈 수 있음
            X += newX;
            Y += newY;
        }

        public void SetPosition(float newX, float newY)
        {
            // 여기에 이동 가능 여부, 속도 제한 등 복잡한 로직이 들어갈 수 있음
            X = newX;
            Y = newY;
        }
    }
}

