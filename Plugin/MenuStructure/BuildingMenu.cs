using Lin.Plugin.ComponentAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lin.Plugin.MenuStructure
{
    public static class BuildingMenu
    {
        /// <summary>
        /// 构建所有菜单
        /// </summary>
        /// <param name="components"></param>
        /// <param name="viewtokens"></param>
        /// <returns></returns>
        public static MenuTabCollection BuildingMenuCollection(List<ComponentStructure> components, List<ViewToken> viewtokens = null)
        {
            MenuTabCollection tabs = new MenuTabCollection();
            if (viewtokens != null)
            {
                tabs.AddRange(GenerViewtoken(viewtokens));
            }
            tabs.AddRange(GenerComponentStructure(components));
            return tabs;
        }

        /// <summary>
        /// 构建早先视图组件的菜单数据结构
        /// </summary>
        /// <param name="viewtokens"></param>
        /// <returns></returns>
        private static MenuTabCollection GenerViewtoken(List<ViewToken> viewtokens)
        {
            viewtokens.Sort(new sortClass());
            MenuTabCollection tabs = new MenuTabCollection();
            List<string> name = new List<string>();
            MenuTab tab = null;
            foreach (ViewToken item in viewtokens)
            {
                if (!name.Contains(item.Menu) && item.Menu != "system")
                {
                    name.Add(item.Menu);
                    tab = new MenuTab();
                    tab.TabName = item.Menu;
                    tabName.Add(tab.TabName);
                    tab.Location = item.Location;
                    if (item.MenuShortcutKey != null)
                    {
                        tab.MenuShortcutKey = item.MenuShortcutKey;
                    }
                    tabs.Add(tab);
                }
            }

            foreach (MenuTab t in tabs)
            {
                foreach (ViewToken v in viewtokens)
                {
                    if (t.TabName == v.Menu)
                    {
                        MenuGroup group = new MenuGroup();
                        group.Name = v.Name;
                        MenuSpecialLargeButton specialButton = new MenuSpecialLargeButton();
                        specialButton.Name = v.Name;
                        specialButton.Icon = v.Icon;
                        specialButton.ViewToken = v;
                        group.Add(specialButton);
                        t.Add(group);
                    }
                }
            }
            return tabs;
        }
        /// <summary>
        /// 从组件数据结构构建菜单数据结构
        /// </summary>
        /// <param name="components"></param>
        /// <returns></returns>
        private static MenuTabCollection GenerComponentStructure(List<ComponentStructure> components)
        {
            foreach (ComponentStructure componentStructure in components)
            {
                object[] largebuttons = componentStructure.ComponentType.GetCustomAttributes(typeof(LargeButton), false);
                if (largebuttons != null && largebuttons.Length > 0)
                {
                    MakeUpMenuLargeButton(largebuttons, componentStructure);
                }

                object[] specialLargebuttons = componentStructure.ComponentType.GetCustomAttributes(typeof(SpecialLargeButton), false);
                if (specialLargebuttons != null && specialLargebuttons.Length > 0)
                {
                    MakeUpSpecialLargeButtons(specialLargebuttons, componentStructure);
                }

                object[] togglebuttons = componentStructure.ComponentType.GetCustomAttributes(typeof(ToggleButton), false);
                if (togglebuttons != null && togglebuttons.Length > 0)
                {
                    MakeUpMenuToggleButton(togglebuttons, componentStructure);
                }

                //....未完（继续将所有按钮类型添加完成）
                //object[] splitbuttons = type.AttributeType.GetCustomAttributes(typeof(SpiltButton), false);
                //object[] dropdownbuttons = type.AttributeType.GetCustomAttributes(typeof(DropDownButton), false);
                //object[] inribbongallerys = type.AttributeType.GetCustomAttributes(typeof(InRibbonGallery), false);
                //object[] comboboxs = type.AttributeType.GetCustomAttributes(typeof(ComponentAttribute.MenuComboBox), false);
                //object[] spinners = type.AttributeType.GetCustomAttributes(typeof(Spinner), false);


                object[] tabActions = componentStructure.ComponentType.GetCustomAttributes(typeof(TabAction), false);
                if (tabActions != null && tabActions.Length > 0)
                {
                    MenuTabAction menuTabAction = null;
                    foreach (object obj in tabActions)
                    {
                        TabAction tabAction = obj as TabAction;
                        if (tabAction != null)
                        {
                            menuTabAction = new MenuTabAction();
                            CopyProperty(tabAction, menuTabAction);
                            menuTabAction.Component = componentStructure;
                            foreach (MenuTab item in Tabs)
                            {
                                if (item.TabName == menuTabAction.TabName)
                                {
                                    item.MenuShortcutKey = menuTabAction.MenuShortKey;
                                    item.Location = menuTabAction.Location;
                                    item.Actions.Add(menuTabAction);
                                    break;
                                }
                            }
                        }
                    }
                }

                object[] groupActions = componentStructure.ComponentType.GetCustomAttributes(typeof(GroupAction), false);
                if (groupActions != null && groupActions.Length > 0)
                {
                    MenuGroupAtion menugroupAction = null;
                    foreach (object obj in groupActions)
                    {
                        GroupAction groupAction = obj as GroupAction;
                        if (groupAction != null)
                        {
                            menugroupAction = new MenuGroupAtion();
                            CopyProperty(groupAction, menugroupAction);
                            menugroupAction.Component = componentStructure;
                            foreach (MenuTab item in Tabs)
                            {
                                foreach (MenuGroup g in item)
                                {
                                    if (item.TabName == menugroupAction.TargetTabName && g.Name == menugroupAction.Name)
                                    {
                                        g.Actions.Add(menugroupAction);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }


            }
            return Tabs;
        }

        /// <summary>
        /// 菜单上所有Tab的名称
        /// </summary>
        private static List<string> tabName = new List<string>();
        /// <summary>
        /// 返回的数据结构
        /// </summary>
        private static MenuTabCollection Tabs = new MenuTabCollection();

        /// <summary>
        /// 从特性类中构建largebutton数据结构
        /// </summary>
        /// <param name="largebuttons"></param>
        /// <returns></returns>
        private static void MakeUpMenuLargeButton(object[] largebuttons, ComponentStructure componentStructure)
        {
            MenuLargeButton menuLargebutton;
            MenuTab tab = null;
            MenuGroup group = null;
            foreach (object obj in largebuttons)
            {
                LargeButton largebutton = obj as LargeButton;
                if (largebutton != null)
                {
                    menuLargebutton = new MenuLargeButton();
                    CopyProperty(largebutton, menuLargebutton);
                    menuLargebutton.Component = componentStructure;

                    if (!tabName.Contains(menuLargebutton.TargetTabName))
                    {
                        tab = new MenuTab();
                        tab.TabName = menuLargebutton.TargetTabName;
                        tabName.Add(tab.TabName);
                        group = new MenuGroup();
                        group.Name = menuLargebutton.TargetGroupName;
                        group.TargetTabName = menuLargebutton.TargetTabName;
                        group.Add(menuLargebutton);
                        tab.Add(group);
                        Tabs.Add(tab);
                    }
                    else
                    {
                        foreach (MenuTab item in Tabs)
                        {
                            if (item.TabName == menuLargebutton.TargetTabName)
                            {
                                bool isHaveGroup = false;
                                foreach (MenuGroup g in item)
                                {
                                    if (g.Name == menuLargebutton.TargetGroupName)
                                    {
                                        isHaveGroup = true;
                                        g.Add(menuLargebutton);
                                        break;
                                    }
                                }
                                if (!isHaveGroup)
                                {
                                    group = new MenuGroup();
                                    group.Name = menuLargebutton.TargetGroupName;
                                    group.TargetTabName = menuLargebutton.TargetTabName;
                                    group.Add(menuLargebutton);
                                    item.Add(group);
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 从特性类中构建SpecialLargeButton数据结构
        /// </summary>
        /// <param name="specialbuttons"></param>
        /// <param name="componentStructure"></param>
        private static void MakeUpSpecialLargeButtons(object[] specialbuttons, ComponentStructure componentStructure)
        {
            MenuSpecialLargeButton menuspecialbutton;
            MenuTab tab = null;
            MenuGroup group = null;
            foreach (object obj in specialbuttons)
            {
                SpecialLargeButton specialbutton = obj as SpecialLargeButton;
                if (specialbutton != null)
                {
                    menuspecialbutton = new MenuSpecialLargeButton();
                    CopyProperty(specialbutton, menuspecialbutton);
                    menuspecialbutton.Component = componentStructure;

                    if (!tabName.Contains(menuspecialbutton.TargetTabName))
                    {
                        tab = new MenuTab();
                        tab.TabName = menuspecialbutton.TargetTabName;
                        tabName.Add(tab.TabName);
                        group = new MenuGroup();
                        group.Name = menuspecialbutton.TargetGroupName;
                        group.TargetTabName = menuspecialbutton.TargetTabName;
                        group.Add(menuspecialbutton);
                        tab.Add(group);
                        Tabs.Add(tab);
                    }
                    else
                    {
                        foreach (MenuTab item in Tabs)
                        {
                            if (item.TabName == menuspecialbutton.TargetTabName)
                            {
                                bool isHaveGroup = false;
                                foreach (MenuGroup g in item)
                                {
                                    if (g.Name == menuspecialbutton.TargetGroupName)
                                    {
                                        isHaveGroup = true;
                                        g.Add(menuspecialbutton);
                                        break;
                                    }
                                }
                                if (!isHaveGroup)
                                {
                                    group = new MenuGroup();
                                    group.Name = menuspecialbutton.TargetGroupName;
                                    group.TargetTabName = menuspecialbutton.TargetTabName;
                                    group.Add(menuspecialbutton);
                                    item.Add(group);
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 从特性类中构建togglebutton数据结构
        /// </summary>
        /// <param name="togglebuttons"></param>
        /// <returns></returns>
        private static void MakeUpMenuToggleButton(object[] togglebuttons, ComponentStructure componentStructure)
        {
            MenuToggleButton menuTogglebutton;
            MenuTab tab = null;
            MenuGroup group = null;
            foreach (object obj in togglebuttons)
            {
                ToggleButton togglebutton = obj as ToggleButton;
                if (togglebutton != null)
                {
                    menuTogglebutton = new MenuToggleButton();
                    CopyProperty(togglebutton, menuTogglebutton);
                    menuTogglebutton.Component = componentStructure;

                    if (!tabName.Contains(menuTogglebutton.TargetTabName))
                    {
                        tab = new MenuTab();
                        tab.TabName = menuTogglebutton.TargetTabName;
                        tabName.Add(tab.TabName);
                        group = new MenuGroup();
                        group.Name = menuTogglebutton.TargetGroupName;
                        group.TargetTabName = menuTogglebutton.TargetTabName;
                        group.Add(menuTogglebutton);
                        tab.Add(group);
                        Tabs.Add(tab);
                    }
                    else
                    {
                        foreach (MenuTab item in Tabs)
                        {
                            if (item.TabName == menuTogglebutton.TargetTabName)
                            {
                                bool isHaveGroup = false;
                                foreach (MenuGroup g in item)
                                {
                                    if (g.Name == menuTogglebutton.TargetGroupName)
                                    {
                                        isHaveGroup = true;
                                        g.Add(menuTogglebutton);
                                        break;
                                    }
                                }
                                if (!isHaveGroup)
                                {
                                    group = new MenuGroup();
                                    group.Name = menuTogglebutton.TargetGroupName;
                                    group.TargetTabName = menuTogglebutton.TargetTabName;
                                    group.Add(menuTogglebutton);
                                    item.Add(group);
                                }
                                break;
                            }
                        }

                    }
                }
            }
        }
        /// <summary>
        /// 从特性类中构建spiltbutton
        /// </summary>
        /// <param name="splitbutton"></param>
        /// <returns></returns>
        private static List<MenuSplitButton> MakeUpMenuSplitButton(object[] splitbutton)
        {

            return null;
        }

        /// <summary>
        /// 把src中的属性值copy到dest的目标属性中
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        public static void CopyProperty(object src, object dest, bool deep = false)
        {
            object tempObj = null;
            object newObj = null;
            if (dest == null)
            {
                return;
            }
            Type type = dest.GetType();
            PropertyInfo[] props = src.GetType().GetProperties();
            PropertyInfo tmp = null;
            foreach (PropertyInfo prop in props)
            {
                tmp = type.GetProperty(prop.Name);
                if (tmp == null)
                {
                    continue;
                }
                if (prop.CanRead && tmp.CanWrite)
                {
                    tempObj = prop.GetValue(src, null);
                    if (tempObj != null && deep)
                    {
                        if (tempObj.GetType().IsClass && !tempObj.GetType().IsPrimitive && tempObj.GetType() != typeof(string))
                        {
                            newObj = Activator.CreateInstance(tempObj.GetType());
                            CopyProperty(tempObj, newObj);
                            tempObj = newObj;
                        }
                    }
                    tmp.SetValue(dest, tempObj, null);
                }
            }
        }


        private class sortClass : Comparer<ViewToken>
        {
            public override int Compare(ViewToken x, ViewToken y)
            {
                Math.Sign(x.Location - y.Location);
                return Math.Sign(x.Location - y.Location);
            }
        };





        /// <summary>
        /// 查找当前目录的所有组件
        /// </summary>
        /// <returns>返回数据结构</returns>
        //internal MenuTabCollection FindComponentold()
        //{
        //    List<AttributeToken> components = attributeStore.FindAttributes(typeof(Component));
        //    Tabs = new MenuTabCollection();
        //    ComponentStructure componentStructure = null;
        //    List<MenuGroup> menugroups = new List<MenuGroup>();
        //    List<MenuLargeButton> menulargebuttons = new List<MenuLargeButton>();
        //    List<MenuToggleButton> menutooglebuttons = new List<MenuToggleButton>();
        //    tabName = new List<string>();
        //    Tabs.AddRange(this.FindViewold());
        //    foreach (AttributeToken type in components)
        //    {
        //        Component component = (Component)type.Attributes;
        //        if (component != null)
        //        {
        //            componentStructure = new ComponentStructure(type.AttributeType);
        //            CopyProperty(component, componentStructure);

        //            object[] largebuttons = type.AttributeType.GetCustomAttributes(typeof(LargeButton), false);
        //            if (largebuttons != null && largebuttons.Length > 0)
        //            {
        //                MakeUpMenuLargeButton(largebuttons, componentStructure);
        //            }

        //            object[] specialLargebuttons = type.AttributeType.GetCustomAttributes(typeof(SpecialLargeButton), false);
        //            if (specialLargebuttons != null && specialLargebuttons.Length > 0)
        //            {
        //                MakeUpSpecialLargeButtons(specialLargebuttons, componentStructure);
        //            }

        //            object[] togglebuttons = type.AttributeType.GetCustomAttributes(typeof(ToggleButton), false);
        //            if (togglebuttons != null && togglebuttons.Length > 0)
        //            {
        //                MakeUpMenuToggleButton(togglebuttons, componentStructure);
        //            }

        //            //....未完（继续将所有按钮类型添加完成）
        //            //object[] splitbuttons = type.AttributeType.GetCustomAttributes(typeof(SpiltButton), false);
        //            //object[] dropdownbuttons = type.AttributeType.GetCustomAttributes(typeof(DropDownButton), false);
        //            //object[] inribbongallerys = type.AttributeType.GetCustomAttributes(typeof(InRibbonGallery), false);
        //            //object[] comboboxs = type.AttributeType.GetCustomAttributes(typeof(ComponentAttribute.MenuComboBox), false);
        //            //object[] spinners = type.AttributeType.GetCustomAttributes(typeof(Spinner), false);

        //            object[] tabActions = type.AttributeType.GetCustomAttributes(typeof(TabAction), false);
        //            if (tabActions != null && tabActions.Length > 0)
        //            {
        //                MenuTabAction menuTabAction = null;
        //                foreach (object obj in tabActions)
        //                {
        //                    TabAction tabAction = obj as TabAction;
        //                    if (tabAction != null)
        //                    {
        //                        menuTabAction = new MenuTabAction();
        //                        CopyProperty(tabAction, menuTabAction);
        //                        menuTabAction.Component = componentStructure;
        //                        foreach (MenuTab item in Tabs)
        //                        {
        //                            if (item.TabName == menuTabAction.TabName)
        //                            {
        //                                item.MenuShortcutKey = menuTabAction.MenuShortKey;
        //                                item.Location = menuTabAction.Location;
        //                                item.Actions.Add(menuTabAction);
        //                                break;
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            object[] groupActions = type.AttributeType.GetCustomAttributes(typeof(GroupAction), false);
        //            if (groupActions != null && groupActions.Length > 0)
        //            {
        //                MenuGroupAtion menugroupAction = null;
        //                foreach (object obj in groupActions)
        //                {
        //                    GroupAction groupAction = obj as GroupAction;
        //                    if (groupAction != null)
        //                    {
        //                        menugroupAction = new MenuGroupAtion();
        //                        CopyProperty(groupAction, menugroupAction);
        //                        menugroupAction.Component = componentStructure;
        //                        foreach (MenuTab item in Tabs)
        //                        {
        //                            foreach (MenuGroup g in item)
        //                            {
        //                                if (item.TabName == menugroupAction.TargetTabName && g.Name == menugroupAction.Name)
        //                                {
        //                                    g.Actions.Add(menugroupAction);
        //                                    break;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return Tabs;
        //}
        /// <summary>
        /// 查找早先的视图
        /// </summary>
        /// <returns>返回数据结构</returns>
        //internal MenuTabCollection FindViewold()
        //{
        //    List<AttributeToken> views = attributeStore.FindAttributes(typeof(ViewAttribute));
        //    ViewToken token = null;
        //    List<ViewToken> tokens = new List<ViewToken>();
        //    foreach (AttributeToken type in views)
        //    {
        //        ViewAttribute view = (ViewAttribute)type.Attributes;
        //        if (view != null)
        //        {
        //            token = new ViewToken(type.AttributeType);
        //            CopyProperty(view, token);
        //            tokens.Add(token);
        //        }
        //    }
        //    tokens.Sort(new sortClass());
        //    MenuTabCollection tabs = new MenuTabCollection();
        //    List<string> name = new List<string>();
        //    MenuTab tab = null;
        //    foreach (ViewToken item in tokens)
        //    {
        //        if (!name.Contains(item.Menu) && item.Menu != "system")
        //        {
        //            name.Add(item.Menu);
        //            tab = new MenuTab();
        //            tab.TabName = item.Menu;
        //            tabName.Add(tab.TabName);
        //            tab.Location = item.Location;
        //            if (item.MenuShortcutKey != null)
        //            {
        //                tab.MenuShortcutKey = item.MenuShortcutKey;
        //            }
        //            tabs.Add(tab);
        //        }
        //    }

        //    foreach (MenuTab t in tabs)
        //    {
        //        foreach (ViewToken v in tokens)
        //        {
        //            if (t.TabName == v.Menu)
        //            {
        //                MenuGroup group = new MenuGroup();
        //                group.Name = v.Name;
        //                MenuSpecialLargeButton specialButton = new MenuSpecialLargeButton();
        //                specialButton.Name = v.Name;
        //                specialButton.Icon = v.Icon;
        //                specialButton.ViewToken = v;
        //                group.Add(specialButton);
        //                t.Add(group);
        //            }
        //        }
        //    }
        //    return tabs;
        //}
    }
}
