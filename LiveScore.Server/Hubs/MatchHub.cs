using LiveScore.Server.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LiveScore.Server.Hubs
{
    public class MatchHub : Hub
    {
        private readonly BasketDbContext _context;

        public async Task UpdateMatch(int matchId,string message)
        {
            await Clients.All.SendAsync("Receive update",message);
        }

        public MatchHub(BasketDbContext context)
        {
            _context = context;
        }

        // Permet � un client de rejoindre un groupe li� � un match
        public async Task JoinMatchGroup(int matchId)
        {
            string groupName = $"match_{matchId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            // Envoyer les �v�nements existants au client
            var events = await _context.EvenementsMatch
                .Where(e => e.MatchId == matchId)
                .OrderBy(e => e.Timestamp)
                .ToListAsync();

            await Clients.Caller.SendAsync("ReceiveAllEvents", events);
        }

        // Permet � un client de quitter un groupe li� � un match
        public async Task LeaveMatchGroup(int matchId)
        {
            string groupName = $"match_{matchId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        // Diffuse un �v�nement � tous les membres d'un groupe
        public async Task BroadcastEvent(int matchId, object eventData)
        {
            string groupName = $"match_{matchId}";
            await Clients.Group(groupName).SendAsync("ReceiveEvent", eventData);
        }
    }
}
