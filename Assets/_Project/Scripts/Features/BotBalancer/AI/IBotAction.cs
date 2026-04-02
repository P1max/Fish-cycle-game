namespace Features.BotBalancer.AI
{
    public interface IBotAction
    {
        // Возвращает число от 0 до 1. Чем больше, тем сильнее бот хочет это сделать.
        float Evaluate(); 
        
        // Само действие
        void Execute();
    }
}