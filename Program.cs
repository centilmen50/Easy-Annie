using System;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Color = System.Drawing.Color;
using SharpDX;
using System.Net;
using System.Text.RegularExpressions;


namespace EasyAnnie
{
    class Program
    {
        public static Spell.Targeted Q;
        public static Spell.Skillshot W;
        public static Spell.Active E;
        public static Spell.Skillshot R;
        public static Menu Menu, FarmingMenu, MiscMenu, DrawMenu, ComboMenu;
        public static List<string> DodgeSpells = new List<string>() { "KatarinaR", "ViR", "NamiR", "xerathrmissilewrapper" };
        static Item Healthpot;
        static Item Manapot;
        static Item CrystalFlask;
        static Item CorruptingPotion;
        static Item RefillablePotion;
        static Item Biscuit;
        static Item Biscuit2;
        static Item Zhonya;
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }

        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Annie")
                return;


            Bootstrap.Init(null);

            Healthpot = new Item(2003, 0);
            Manapot = new Item(2004, 0);
            CrystalFlask = new Item(2041, 0);
            CorruptingPotion = new Item(2033, 0);
            RefillablePotion = new Item(2031, 0);
            Biscuit = new Item(2009, 0);
            Biscuit2 = new Item(2010, 0);
            Zhonya = new Item(3157, 0);

            Q = new Spell.Targeted(SpellSlot.Q, 625);
            W = new Spell.Skillshot(SpellSlot.W, 625, SkillShotType.Cone, 250, 1600, 50);
            E = new Spell.Active(SpellSlot.E);
            R = new Spell.Skillshot(SpellSlot.R, 600, SkillShotType.Circular, 250, 2000, 200);

            Menu = MainMenu.AddMenu("Easy Annie", "easyannie");
            
            ComboMenu = Menu.AddSubMenu("Combo","ComboSettings");            
            ComboMenu.AddLabel("Combo Settings");
            ComboMenu.Add("Auto Ignite", new CheckBox("Auto Ignite",false));
            ComboMenu.Add("Save Stun", new CheckBox("Save Stun",false));
            ComboMenu.Add("R Count", new Slider("Min R Count >= ", 2, 1, 5));

            FarmingMenu = Menu.AddSubMenu("Farm", "FarmSettings");
            FarmingMenu.AddLabel("Lane Clear");
            FarmingMenu.Add("QLaneClear", new CheckBox("Use Q"));
            FarmingMenu.Add("WLaneClear", new CheckBox("Use W"));

            FarmingMenu.AddLabel("Last Hit");
            FarmingMenu.Add("Qlasthit", new CheckBox("Use Q"));           

            MiscMenu = Menu.AddSubMenu("Misc", "Misc");
            MiscMenu.AddLabel("KillSteal");
            MiscMenu.Add("Qkill", new CheckBox("Use Q"));
            MiscMenu.Add("Wkill", new CheckBox("Use W"));
            MiscMenu.Add("Rkill", new CheckBox("Use R"));

            MiscMenu.AddLabel("Activator");
            MiscMenu.Add("useHP", new CheckBox("Use Health Potion"));           
            MiscMenu.Add("useHPV", new Slider("HP < %", 45, 0, 100));
            MiscMenu.Add("useMana", new CheckBox("Use Mana Potion"));
            MiscMenu.Add("useManaV", new Slider("Mana < %", 45, 0, 100));
            MiscMenu.Add("useCrystal", new CheckBox("Use New Potions"));
            MiscMenu.Add("useCrystalHPV", new Slider("HP < %", 45, 0, 100));
            MiscMenu.Add("useCrystalManaV", new Slider("Mana < %", 45, 0, 100));
            MiscMenu.Add("useZhonya", new CheckBox("Use Zhonya"));

            DrawMenu = Menu.AddSubMenu("Draw", "Drawings");
            DrawMenu.Add("drawAA", new CheckBox("Draw AA"));
            DrawMenu.Add("drawQ", new CheckBox("Draw Q"));
            DrawMenu.Add("drawE", new CheckBox("Draw W"));
            DrawMenu.Add("drawR", new CheckBox("Draw R"));

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
        }

        static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (DodgeSpells.Any(el => el == args.SData.Name))
            {
                if (Q.IsReady() && Q.IsInRange(sender) && _Player.HasBuff("pyromania_particle"))
                {
                    Q.Cast(sender);
                }
                else if (R.IsReady() && R.IsInRange(sender) && _Player.HasBuff("pyromania_particle"))
                {
                    R.Cast(sender);
                }

            }
        }


        private static void Game_OnTick(EventArgs args)
        {
            var HPpot = MiscMenu["useHP"].Cast<CheckBox>().CurrentValue;
            var Mpot = MiscMenu["useMana"].Cast<CheckBox>().CurrentValue;
            var Crystal = MiscMenu["useCrystal"].Cast<CheckBox>().CurrentValue;
            var HPv = MiscMenu["useHPv"].Cast<Slider>().CurrentValue;
            var Manav = MiscMenu["useManav"].Cast<Slider>().CurrentValue;
            var CrystalHPv = MiscMenu["useCrystalHPv"].Cast<Slider>().CurrentValue;
            var CrystalManav = MiscMenu["useCrystalManav"].Cast<Slider>().CurrentValue;
            var igntarget = TargetSelector.GetTarget(600, DamageType.True);
            

            if (HPpot && Player.Instance.HealthPercent < HPv)
            {
                if (Item.HasItem(Healthpot.Id) && Item.CanUseItem(Healthpot.Id) && !Player.HasBuff("RegenerationPotion"))
                {
                    Healthpot.Cast();
                }
                if (Item.HasItem(Biscuit.Id) && Item.CanUseItem(Biscuit.Id) && !Player.HasBuff("RegenerationPotion"))
                {
                    Biscuit.Cast();
                }
                if (Item.HasItem(Biscuit2.Id) && Item.CanUseItem(Biscuit2.Id) && !Player.HasBuff("RegenerationPotion"))
                {
                    Biscuit2.Cast();
                }
            }
            
            if (Crystal && Player.Instance.HealthPercent < CrystalHPv || Crystal && Player.Instance.ManaPercent < CrystalManav)
            {
                if (Item.HasItem(CorruptingPotion.Id) && Item.CanUseItem(CorruptingPotion.Id) && !Player.HasBuff("RegenerationPotion") && !Player.HasBuff("FlaskOfCrystalWater") && !Player.HasBuff("ItemCrystalFlask"))
                {
                    CorruptingPotion.Cast();
                }
               
            }

            if (Crystal && Player.Instance.HealthPercent < CrystalHPv || Crystal && Player.Instance.ManaPercent < CrystalManav)
            {
                if (Item.HasItem(RefillablePotion.Id) && Item.CanUseItem(RefillablePotion.Id) && !Player.HasBuff("RegenerationPotion") && !Player.HasBuff("FlaskOfCrystalWater") && !Player.HasBuff("ItemCrystalFlask"))
                {
                    RefillablePotion.Cast();
                }

            }


            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }
            KillSteal();
            Auto();

            


        }
        static void Auto()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            foreach (AIHeroClient enemie in EntityManager.Heroes.Enemies)
            {
                if (Player.Instance.IsDead || !Player.Instance.CanCast || Player.Instance.IsInvulnerable || !Player.Instance.IsTargetable || Player.Instance.IsZombie || Player.Instance.IsInShopRange())
                {
                    return;
                }              
                else if (Player.Instance.HealthPercent < 10 && EntityManager.Heroes.Enemies.Where(enemy => enemy != _Player && enemy.Distance(_Player) <= 800).Count() >= 1 && !target.IsDead && !target.IsZombie && enemie.CanCast)
                {                   
                    Zhonya.Cast();
                }

            }
        }
        private static void Combo()
        {
            
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var rCount = ComboMenu["R Count"].Cast<Slider>().CurrentValue;
            
            if (Q.IsReady() && target.IsValidTarget(Q.Range) && !target.IsDead)
            {
                Q.Cast(target);
            }
            if (W.IsReady() && target.IsValidTarget(W.Range) && !target.IsDead)
            {
                W.Cast(target);
            }
            if (E.IsReady() && target.IsValidTarget(Q.Range))
            {
                E.Cast();
            }
            
            foreach (AIHeroClient enemie in EntityManager.Heroes.Enemies)
            {
                if (R.IsReady() && EntityManager.Heroes.Enemies.Where(enemy => enemy != _Player && enemy.Distance(_Player) <= R.Range).Count() >= rCount && !target.IsDead)
                {
                    R.Cast(enemie);
                }
                
            }
        }
        private static void KillSteal()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var useQ = MiscMenu["Qkill"].Cast<CheckBox>().CurrentValue;
            var useW = MiscMenu["Wkill"].Cast<CheckBox>().CurrentValue;
            var useR = MiscMenu["Rkill"].Cast<CheckBox>().CurrentValue;

            if (Q.IsReady() && useQ && target.IsValidTarget(Q.Range)  && target.Health <= _Player.GetSpellDamage(target, SpellSlot.Q))
            {
                Q.Cast(target);
            }
            if (W.IsReady() && useW && target.IsValidTarget(W.Range)  && target.Health <= _Player.GetSpellDamage(target, SpellSlot.W))
            {
                W.Cast(target);
            }
            if (R.IsReady() && useR && target.IsValidTarget(R.Range) && target.Health <= _Player.GetSpellDamage(target, SpellSlot.R))
            {
                R.Cast(target);
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (Q.IsReady() && target.IsValidTarget(Q.Range) && !target.IsDead )
            {
                Q.Cast(target);
            }

        }
        private static void LaneClear()
        {
            var stun = ComboMenu["Save Stun"].Cast<CheckBox>().CurrentValue;
            if (stun && _Player.HasBuff("pyromania_particle"))
            {
                return;
            }
            var useQ = FarmingMenu["QLaneClear"].Cast<CheckBox>().CurrentValue;
            var useW = FarmingMenu["WLaneClear"].Cast<CheckBox>().CurrentValue;
            var minions = ObjectManager.Get<Obj_AI_Base>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady() && minion.IsValidTarget(Q.Range) && minion.Health < _Player.GetSpellDamage(minion, SpellSlot.Q))
                {
                    Q.Cast(minion);
                }
                if (useW && W.IsReady() && Player.Instance.ManaPercent > 35 && minion.IsValidTarget(W.Range) && minions.Count() >= 3)
                {
                    W.Cast(minion);
                }
            }
        }
        private static void LastHit()
        {
            var stun = ComboMenu["Save Stun"].Cast<CheckBox>().CurrentValue;
            if (stun && _Player.HasBuff("pyromania_particle"))
            {
                return;
            }
            var useQ = FarmingMenu["Qlasthit"].Cast<CheckBox>().CurrentValue;
            var minions = ObjectManager.Get<Obj_AI_Base>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            foreach (var minion in minions)
            {
                if (useQ && Q.IsReady() && minion.IsValidTarget(Q.Range) && minion.Health < _Player.GetSpellDamage(minion, SpellSlot.Q))
                {
                    Q.Cast(minion);
                }
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Red, BorderWidth = 1, Radius = Q.Range }.Draw(_Player.Position);
            }
            if (DrawMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Red, BorderWidth = 1, Radius = W.Range }.Draw(_Player.Position);
            }
            if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Red, BorderWidth = 1, Radius = R.Range }.Draw(_Player.Position);
            }
        }
    }
}