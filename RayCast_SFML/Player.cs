
namespace RayCast_SFML
{
    internal class Player
    {
        //Поля
        public double Fov; //Поле зрения
        public double HalfFov; //Половина поля зрения
        public double X; //Координата X 
        public double Y; //Координата Y
        public double Angle; //Угол зрения
        public double MoveSpeed; //Скорость передвижения
        public double RotationSpeed; //Скорость поворота
        public double Radius; //Радиус

        public Player()
        {
            Fov = 60;
            HalfFov = Fov / 2;
            X = 3;
            Y = 3;
            Angle = 45;
            MoveSpeed = 4.0;
            RotationSpeed = 60.0;
            Radius = 4;
        }
    }
}
