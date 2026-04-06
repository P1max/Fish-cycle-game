namespace Features.BotBalancer.AI
{
    public interface IBotAction
    {
        float Evaluate(); 
        
        void Execute();
    }
}