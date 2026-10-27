using System;
using System.Collections.Generic;
using System.Text;
using PlatformerGame.PlayerCharacters;

namespace PlatformerGame.Interfaces
{
    public interface ICollectable
    {
        bool IsCollected { get; set; }
        void Collect(Player p);
    }
}
