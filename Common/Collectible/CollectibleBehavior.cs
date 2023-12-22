﻿using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

namespace Vintagestory.API.Common
{
    public abstract class CollectibleBehavior
    {
        /// <summary>
        /// The collectible object (item or block) for this behavior instance.
        /// </summary>
        public CollectibleObject collObj;

        /// <summary>
        /// The properties of this block behavior.
        /// </summary>
        public string propertiesAtString;

        /// <summary>
        /// If true, this behavior is not required on the client. This is here because copygirl doesn't stop asking for it. Probably breaks things. If it breaks things, complain to copygirl please :p
        /// </summary>
        public virtual bool ClientSideOptional => false;

        public CollectibleBehavior(CollectibleObject collObj)
        {
            this.collObj = collObj;
        }

        /// <summary>
        /// Called right after the block behavior was created, must call base method
        /// </summary>
        /// <param name="properties"></param>
        public virtual void Initialize(JsonObject properties)
        {
            this.propertiesAtString = properties.ToString();
        }

        /// <summary>
        /// Server Side: Called once the collectible has been registered
        /// Client Side: Called once the collectible has been loaded from server packet
        /// </summary>
        /// <param name="api"></param>
        public virtual void OnLoaded(ICoreAPI api)
        {

        }

        public virtual void OnUnloaded(ICoreAPI api)
        {

        }

        public virtual EnumItemStorageFlags GetStorageFlags(ItemStack itemstack, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
            return 0;
        }

        /// <summary>
        /// When the player has begun using this item for attacking (left mouse click). Return true to play a custom action.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSel"></param>
        /// <param name="entitySel"></param>
        /// <param name="handHandling"></param>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual void OnHeldAttackStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, ref EnumHandHandling handHandling, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
        }


        /// <summary>
        /// When the player has canceled a custom attack action. Return false to deny action cancellation.
        /// </summary>
        /// <param name="secondsPassed"></param>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSelection"></param>
        /// <param name="entitySel"></param>
        /// <param name="cancelReason"></param>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual bool OnHeldAttackCancel(float secondsPassed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSelection, EntitySelection entitySel, EnumItemUseCancelReason cancelReason, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
            return false;
        }

        /// <summary>
        /// Called continously when a custom attack action is playing. Return false to stop the action.
        /// </summary>
        /// <param name="secondsPassed"></param>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSelection"></param>
        /// <param name="entitySel"></param>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual bool OnHeldAttackStep(float secondsPassed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSelection, EntitySelection entitySel, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
            return false;
        }

        /// <summary>
        /// Called when a custom attack action is finished
        /// </summary>
        /// <param name="secondsPassed"></param>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSelection"></param>
        /// <param name="entitySel"></param>
        /// <param name="handling"></param>
        public virtual void OnHeldAttackStop(float secondsPassed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSelection, EntitySelection entitySel, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
        }


        /// <summary>
        /// Called when the player right clicks while holding this block/item in his hands
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSel"></param>
        /// <param name="entitySel"></param>
        /// <param name="firstEvent"></param>
        /// <param name="handHandling">Whether or not to do any subsequent actions. If not set or set to NotHandled, the action will not called on the server.</param>
        /// <param name="handling">Set to PreventDefault to not try eating the item, set to PreventSubsequent to ignore any subsequent calls to OnHeldInteractStart() of other behaviors</param>
        /// <returns></returns>
        public virtual void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handHandling, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
        }


        /// <summary>
        /// Called every frame while the player is using this collectible
        /// </summary>
        /// <param name="secondsUsed"></param>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSel"></param>
        /// <param name="entitySel"></param>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual bool OnHeldInteractStep(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
            return false;
        }

        /// <summary>
        /// Called when the player successfully completed the using action, not called when successfully cancelled
        /// </summary>
        /// <param name="secondsUsed"></param>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSel"></param>
        /// <param name="entitySel"></param>
        /// <param name="handling"></param>
        public virtual void OnHeldInteractStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
            return;
        }


        /// <summary>
        /// When the player released the right mouse button. Return false to deny the cancellation (= will keep using the item until OnHeldInteractStep returns false).
        /// </summary>
        /// <param name="secondsUsed"></param>
        /// <param name="slot"></param>
        /// <param name="byEntity"></param>
        /// <param name="blockSel"></param>
        /// <param name="entitySel"></param>
        /// <param name="cancelReason"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        public virtual bool OnHeldInteractCancel(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, EnumItemUseCancelReason cancelReason, ref EnumHandling handled)
        {
            return true;
        }

        /// <summary>
        /// Called when the collectible is rendered in hands, inventory or on the ground
        /// </summary>
        /// <param name="capi"></param>
        /// <param name="itemstack"></param>
        /// <param name="target"></param>
        /// <param name="renderinfo"></param>
        public virtual void OnBeforeRender(ICoreClientAPI capi, ItemStack itemstack, EnumItemRenderTarget target, ref ItemRenderInfo renderinfo)
        {
            
        }


        /// <summary>
        /// Interaction help that is shown when selecting the item in the hotbar slot
        /// </summary>
        /// <param name="inSlot"></param>
        /// <param name="handling"></param>
        /// <returns></returns>
        public virtual WorldInteraction[] GetHeldInteractionHelp(ItemSlot inSlot, ref EnumHandling handling)
        {
            handling = EnumHandling.PassThrough;
            return new WorldInteraction[0];
        }

        /// <summary>
        /// Called when the tool mode (F) key is pressed to generate the GUI
        /// </summary>
        public virtual SkillItem[] GetToolModes(ItemSlot slot, IClientPlayer forPlayer, BlockSelection blockSel)
        {
            return null;
        }

        /// <summary>
        /// Should return the current items tool mode.
        /// </summary>
        public virtual int GetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSelection)
        {
            return 0;
        }

        /// <summary>
        /// Should set given toolmode
        /// </summary>
        public virtual void SetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSelection, int toolMode)
        {
        }


        public virtual void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {

        }

        public virtual void GetHeldItemName(StringBuilder sb, ItemStack itemStack)
        {
            
        }

        /// <summary>
        /// Player has broken a block while holding this collectible. Return false if you want to cancel the block break event.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="byEntity"></param>
        /// <param name="itemslot"></param>
        /// <param name="blockSel"></param>
        /// <param name="dropQuantityMultiplier"></param>
        /// <param name="bhHandling"></param>
        public virtual bool OnBlockBrokenWith(IWorldAccessor world, Entity byEntity, ItemSlot itemslot, BlockSelection blockSel, float dropQuantityMultiplier, ref EnumHandling bhHandling)
        {
            bhHandling = EnumHandling.PassThrough;
            return true;
        }


        /// <summary>
        /// Player is holding this collectible and breaks the targeted block
        /// </summary>
        /// <param name="player"></param>
        /// <param name="blockSel"></param>
        /// <param name="itemslot"></param>
        /// <param name="remainingResistance"></param>
        /// <param name="dt"></param>
        /// <param name="counter"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        public virtual float OnBlockBreaking(IPlayer player, BlockSelection blockSel, ItemSlot itemslot, float remainingResistance, float dt, int counter, ref EnumHandling handled)
        {
            handled = EnumHandling.PassThrough;
            return remainingResistance;
        }
    }
}