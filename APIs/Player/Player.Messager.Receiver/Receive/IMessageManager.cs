using System.Threading.Tasks;

namespace Player.Messager.Receiver
{
    public interface IMessageManager
    {
        Task Handler(string content, string routingKey); 
    }
}