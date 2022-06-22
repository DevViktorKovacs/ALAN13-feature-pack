using ALAN13featurepack.Interfaces;
using ALAN13featurepack.Utility;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAN13featurepack.GameWorld
{
    public class Structure
    {
        public StructureType StructureType { get; set; }

        public Sprite Shadow { get; set; }

        public AnimatedSprite StructureSprites { get; set; }

        public WorldOrientation Orientation { get; set; }

        public Dictionary<WorldOrientation, Vector2> DirectionalOffset { get; set; }

        public AudioStreamPlayer2D AudioStreamPlayer { get; set; }

        public Sprite Ornament { get; set; }

        public Area2D Area { get; set; }

        public Node2D Content { get; set; }

        public int ID { get; set; }

        public Vector2 GridPosition { get; set; }

        public bool Visible
        {
            get
            {
                return visible;
            }

            set
            {
                if (Shadow != null) Shadow.Visible = value;

                StructureSprites.Visible = value;

                visible = value;
            }
        }

        private bool visible = true;

        public float AnimationSpeed
        {
            get
            {
                return animationSpeed;
            }
            set
            {
                animationSpeed = value;

                StructureSprites.SpeedScale = animationSpeed;
            }
        }

        float animationSpeed = 1;
        public TileCell TileCell { get; set; }

        public override string ToString()
        {
            return $"{StructureType}: {Orientation}";
        }

        public string ToStringDetailed()
        {
            return $"SID:{ID} @({GridPosition}) T: {StructureType}: {Orientation}";
        }

        public TileCell GetTileCell()
        {
            return TileCell;
        }

        public void SetPosition(Vector2 gridPosition)
        {
            GridPosition = gridPosition;

            if (Shadow != default) Shadow.Position = Shadow.GetTileGrid(TileCell.TileGridId).GetCellCenterAtGridPosition(gridPosition);

            if (StructureSprites != default) StructureSprites.Position = StructureSprites.GetTileGrid(TileCell.TileGridId).GetCellCenterAtGridPosition(gridPosition);

            if (Area != default) Area.Position = StructureSprites.GetTileGrid(TileCell.TileGridId).GetCellCenterAtGridPosition(gridPosition);
        }
    }
}
