﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RimWorld;
using Verse;

namespace BlocksMenu
{
    public static class GeneralUI
    {
        public static int ScrollMenu<T>(Rect rectOut, Action<Rect, T> decorateButton, List<T> list, ref Vector2 scrollPosition, float buttonHeight = 30f, bool doMouseoverSound = false)
        {
            int selectedItem = -1;

            Rect rectView = new Rect(0f, 0f, rectOut.width - 16f, list.Count * buttonHeight);
            Widgets.BeginScrollView(rectOut, ref scrollPosition, rectView);

            // only draw button if it is in view otherwise just forget about it
            int firstButtonInView = (int)(scrollPosition.y / buttonHeight);
            int lastButtonInView = (int)((scrollPosition.y + rectOut.height) / buttonHeight) + 1; // +1 for margin of error
            for (int i = firstButtonInView; i < list.Count && i < lastButtonInView; i++)
            {
                Rect rectButt = new Rect(0f, i * buttonHeight, rectView.width, buttonHeight);
                decorateButton(rectButt, list[i]);
                if (Widgets.ButtonText(rectButt, "", false, doMouseoverSound))
                    selectedItem = i;
            }

            Widgets.EndScrollView();

            // will return item from list if button has been clicked
            return selectedItem;
        }

        // better one than above i think
        public static bool ScrollMenu<T>(Rect rectOut, Action<Rect, T> decorateButton, List<T> list, ref T selectedItem, ref Vector2 scrollPosition, float buttonHeight = 30f, bool doMouseoverSound = false)
        {
            bool buttonPressed = false;

            Rect rectView = new Rect(0f, 0f, rectOut.width - 16f, list.Count * buttonHeight);
            Widgets.BeginScrollView(rectOut, ref scrollPosition, rectView);


            // only draw button if it is in view otherwise just forget about it
            int firstButtonInView = (int)(scrollPosition.y / buttonHeight);
            int lastButtonInView = (int)((scrollPosition.y + rectOut.height) / buttonHeight) + 1; // +1 for margin of error
            for (int i = firstButtonInView; i < list.Count && i < lastButtonInView; i++)
            {
                Rect rectButt = new Rect(0f, i * buttonHeight, rectView.width, buttonHeight);
                decorateButton(rectButt, list[i]);
                if (Widgets.ButtonText(rectButt, "", false, doMouseoverSound))
                {
                    buttonPressed = true;
                    selectedItem = list[i];
                }
            }

            Widgets.EndScrollView();

            return buttonPressed;
        }

        public static bool SearchBar(Rect rectView, ref string searchString)
        {
            bool update = false;

            // scroll rects
            Rect textBox = new Rect(
                rectView.x,
                rectView.y,
                rectView.width - rectView.height,
                rectView.height);
            Rect buttonClear = new Rect(
                textBox.x + textBox.width,
                rectView.y,
                rectView.height,
                rectView.height);

            if (Widgets.ButtonImage(buttonClear.ContractedBy(3f), ContentFinder<Texture2D>.Get("UI/Widgets/CloseXSmall"), Color.white, Color.white * GenUI.SubtleMouseoverColor, true))
            {
                //Verse.Sound.SoundStarter.PlayOneShotOnCamera(SoundDefOf.Click);
                searchString = "";
                update = true;
            }

            // need to give the textbox a name inorder to do the loose focus thing below
            string textBoxName = "search";
            GUI.SetNextControlName(textBoxName);

            // draw textbox and make sure to store the string entered in the box
            searchString = Widgets.TextField(textBox, searchString);

            bool focused = GUI.GetNameOfFocusedControl() == textBoxName;

            // loose focus so the filter does not always run
            if (Input.GetMouseButtonDown(0) && !Mouse.IsOver(textBox) && focused)
                GUI.FocusControl(null);

            // only update if focused and the user presses a keyboard key
            if (focused && Event.current.isKey)
                update = true;

            // gives you info on if the textbox has been updated
            return update;
        }

        // not sure if this is stupid or not but i like it here
        public static Dialog_BillConfig OpenDialogBillConfig(RecipeDef recipe, ref Bill_Production bill)
        {
            if (recipe == null)
                return null;

            // open bill config menu
            Building_WorkTable tempTable = new Building_WorkTable();
            bill = new Bill_Production(recipe);
            Dialog_BillConfig billConfig = new Dialog_BillConfig(bill, tempTable.Position);
            // note: need to add bill to temp table or bill config will not open properly
            tempTable.billStack.AddBill(bill);
            Find.WindowStack.Add(billConfig);

            return billConfig;
        }

        public static Rect LabelColorAndOutLine(Rect rect, string label, Color color, TextAnchor anchor, float margin = 0f)
        {
            // color outline
            GUI.color = color;
            Widgets.DrawBox(rect);

            // reset color
            GUI.color = Color.white;

            // color in rect to what ever this sets the color to
            Widgets.DrawAltRect(rect);

            rect = rect.ContractedBy(margin);

            Text.Anchor = anchor;
            Widgets.Label(rect, label);

            // reset anchor
            Text.Anchor = TextAnchor.UpperLeft;

            // set rect size after label is in
            rect.height -= 25f;
            rect.y += 25f;

            Widgets.DrawLineHorizontal(rect.x, rect.y - 5f, rect.width);

            return rect;
        }

        public static bool CheckboxMinimal(Rect rect, string description, Color color, ref bool currentState, bool highlightWhenTrue = true, float margin = 2f)
        {
            GUI.color = color;
            Widgets.DrawBox(rect);
            GUI.color = Color.white;

            if (description != "")
                TooltipHandler.TipRegion(rect, new TipSignal(description));

            if (currentState == highlightWhenTrue)
                Widgets.DrawBoxSolid(rect.ContractedBy(margin), color);

            if (Widgets.ButtonText(rect, "", false, false))
            {
                currentState = !currentState;
                return true;
            }

            return false;
        }
    }
}
