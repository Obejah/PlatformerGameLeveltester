using PlatformerGame.Entities;
using PlatformerGame.Enums;
using PlatformerGame.Interfaces;
using PlatformerGame.Level;
using PlatformerGame.PlayerCharacters; // For Player
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using System.Windows;

namespace PlatformerGame.Level
{
    public class aLevel
    {
        public string Name { get; private set; }
        public int Screens { get; private set; }
        public int TileSize { get; private set; }

        public List<Entity> Entities { get; private set; } = new List<Entity>();
        public List<Platform> Platforms { get; private set; } = new List<Platform>();
        public List<Collectable> Collectables { get; private set; } = new List<Collectable>();
        public Vector SpawnPoint { get; private set; }
        public Vector ActiveCheckpoint { get; set; }

        public Checkpoint LevelCheckpoint { get; set; }

        public int FossilsCollected { get; set; }
        public int FossilsTotal { get; private set; }

        public aLevel(LevelData data)
        {
            Name = data.Name;
            Screens = data.Screens;
            TileSize = data.TileSize;
            SpawnPoint = data.PlayerStart;
            ActiveCheckpoint = SpawnPoint;
            FossilsTotal = data.FossilCount;

            int nextId = 1;
            foreach (var entData in data.Entities)
            {
                Entity newEntity = CreateEntityFromData(entData, nextId++);
                if (newEntity != null)
                {
                    AddEntity(newEntity);
                }
            }
        }

        public void AddEntity(Entity e)
        {
            Entities.Add(e);

            if (e is Platform p) Platforms.Add(p);
            if (e is Collectable c) Collectables.Add(c);
           if (e is Checkpoint cp) LevelCheckpoint= cp;
        }

        public void Update(float dt)
        {
            foreach (var entity in Entities)
            {
                if (entity.Active)
                {
                    entity.Update(dt);
                }
            }

        }

        public List<Entity> GetEntitiesIn(Rect bounds)
        {
            return Entities.Where(e => e.Active && e.GetBounds().IntersectsWith(bounds)).ToList();
        }

        public Vector GetSpawnPoint()
        {
            return ActiveCheckpoint;
        }


        private Entity CreateEntityFromData(EntityData data, int id)
        {
            Vector pos = new Vector(data.X, data.Y);
            Vector size = new Vector(data.Width, data.Height);

            if (!Enum.TryParse(data.Type, true, out EntityType type))
            {
                Console.WriteLine($"Warning: Unknown entity type '{data.Type}'");
                return null;
            }

            switch (type)
            {
                case EntityType.Ground:
                    return new Ground(id, pos, size);
                case EntityType.Platform:
                    return new SolidPlatform(id, pos, size);
                case EntityType.OneWayPlatform:
                    return new OneWayPlatform(id, pos, size);
                case EntityType.LevelStart:
                    return new LevelStart(id, pos, size);
                case EntityType.Checkpoint:
                    return new Checkpoint(id, pos, size);
                case EntityType.LevelEnd:
                    return new LevelEnd(id, pos, size);
                case EntityType.Enemy:
                case EntityType.Fossil:
                    return new Fossil(id, pos, size, 1, false);
                case EntityType.Spikes:
                    return new Spikes(id, pos, size, 1); // 1 damage
                default:
                    Console.WriteLine($"Warning: Entity type '{type}' not implemented yet.");
                    return null;
            }
        }

        public void SetActiveCheckpoint(Checkpoint cp)
        {
            if (LevelCheckpoint != null)
            {
                LevelCheckpoint = cp;
            }
            ActiveCheckpoint = cp.Position;
        }

        public void Reset()
        {
            FossilsCollected = 0;
            ActiveCheckpoint = SpawnPoint;

            foreach (var entity in Entities)
            {
                entity.Active = true; 
            }
        }
    }
}