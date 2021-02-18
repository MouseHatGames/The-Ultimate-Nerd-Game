// after much debate I've decided not to use this for now, and continue with the GameplayUIState enum. But I may change my mind in the future.

namespace UnusedCode
{
    public abstract class GameplayUIState<T> where T : class, new() // I have absolutely no idea what this line of code does. It's from stack overflow. inherit like so: public class None : GameplayUIState<None> {
    {
        public abstract void Initialize();

        public abstract void Run();

        public abstract void Finish();


        // singleton stuff
        private static T privateinstance;
        public static T GetInstance()
        {
            if (privateinstance == null)
            {
                privateinstance = new T();
            }
            return privateinstance;
        }
    }
}