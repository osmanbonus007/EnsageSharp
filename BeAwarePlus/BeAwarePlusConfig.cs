using System;

using BeAwarePlus.Data;
using BeAwarePlus.GlobalList;
using BeAwarePlus.Checker;
using BeAwarePlus.Menus;
using BeAwarePlus.ParticleChecker;
using BeAwarePlus.Renderer;

namespace BeAwarePlus
{
    internal class BeAwarePlusConfig : IDisposable
    {
        public BeAwarePlusConfig(BeAwarePlus BeAwarePlus)
        {
            MenuManager = new MenuManager();

            Colors = new Colors();

            Dangerous = new Dangerous();

            EntityToTexture = new EntityToTexture();

            ModifierToTexture = new ModifierToTexture();

            ParticleToTexture = new ParticleToTexture();

            GlobalMiniMap = new GlobalMiniMap();

            GlobalWorld = new GlobalWorld();

            MessageCreator = new MessageCreator(MenuManager);

            Resolution = new Resolution(MessageCreator);

            SoundPlayer = new SoundPlayer(MenuManager);

            Others = new Others(
                MenuManager,
                BeAwarePlus.Context.Owner,
                MessageCreator,
                SoundPlayer);

            DrawHelper = new DrawHelper(
                MenuManager,
                BeAwarePlus.Context.Owner,
                GlobalMiniMap,
                GlobalWorld);

            ParticleSpells = new ParticleSpells(
                MenuManager,
                BeAwarePlus.Context.Owner,
                Dangerous,
                MessageCreator,
                SoundPlayer,
                Colors,
                DrawHelper);

            ParticleItems = new ParticleItems(
                MenuManager,
                BeAwarePlus.Context.Owner,
                Dangerous,
                MessageCreator,
                SoundPlayer,
                Colors,
                DrawHelper);

            ParticleTeleport = new ParticleTeleport(
                MenuManager,
                BeAwarePlus.Context.Owner,
                MessageCreator,
                SoundPlayer,
                Colors,
                GlobalMiniMap,
                GlobalWorld);

            Entities = new Entities(
                MenuManager,
                BeAwarePlus.Context.Owner,
                Dangerous,
                MessageCreator,
                SoundPlayer,
                Colors,
                GlobalMiniMap,
                GlobalWorld);

            Modifiers = new Modifiers(
                MenuManager,
                BeAwarePlus.Context.Owner,
                Dangerous,
                MessageCreator,
                SoundPlayer,
                Colors,
                GlobalMiniMap,
                GlobalWorld);

            OnMiniMap = new OnMiniMap(
                MenuManager,
                BeAwarePlus.Render, 
                GlobalMiniMap);

            OnWorld = new OnWorld(
                MenuManager,
                GlobalWorld,
                ParticleToTexture);
        }

        public MenuManager MenuManager { get; set; }

        public Colors Colors { get; }

        private Dangerous Dangerous { get; }

        public EntityToTexture EntityToTexture { get; }

        public ModifierToTexture ModifierToTexture { get; }

        public ParticleToTexture ParticleToTexture { get; }

        private GlobalMiniMap GlobalMiniMap { get; }

        private GlobalWorld GlobalWorld { get; }

        private OnMiniMap OnMiniMap { get; }

        private Resolution Resolution { get; }

        public MessageCreator MessageCreator { get; }

        public SoundPlayer SoundPlayer { get; }

        private Others Others { get; }

        public ParticleSpells ParticleSpells { get; }

        public ParticleItems ParticleItems { get; }

        public ParticleTeleport ParticleTeleport { get; }

        public Entities Entities { get; }

        public Modifiers Modifiers { get; }

        private DrawHelper DrawHelper { get; }

        private OnWorld OnWorld { get; }

        private bool Disposed { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }

            if (disposing)
            {
                OnMiniMap.Dispose();
                OnWorld.Dispose();
                Others.Dispose();
                MenuManager.Factory.Dispose();
            }

            Disposed = true;
        }
    }
}
