   M S B A M L   `   `   `  ���� G  CBimEraPlugin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:BimEraPlugin.ViewsBimEraPlugin.Views  -    'BimEraPlugin.Views.CreateFixturesWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     _ [MaterialDesignThemes.Wpf, Version=4.3.0.3, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  ,local clr-namespace:BimEraPlugin.Views    � Title$  CreateFixturesWindow��$	��275��$	��550�� � WindowStartupLocation$ CenterScreen=� � 	Resources -   'BimEraPlugin.Views.CreateFixturesWindow �� 
TargetType �� 	Resources �� MergedDictionaries �� Source �� Property �� Value   PrimaryHueMidBrush  MaterialDesignPaper	 �� 
TargetType(  "MaterialDesignThemes.Wpf.ColorZone
  Name   Mode+  %MaterialDesignThemes.Wpf.ShadowAssist   ShadowDepth � ColumnDefinitions $  MaterialDesignHeadline5TextBlock �� Orientation'  !System.Windows.Shell.WindowChrome   IsHitTestVisibleInChrome'  !MaterialDesignThemes.Wpf.PackIcon   Kind)  #MaterialDesignThemes.Wpf.DialogHost   CloseOnClickAway   
Identifier � RowDefinitions �� DisplayMemberPath �� SelectedItem'  !MaterialDesignThemes.Wpf.PopupBox   	StaysOpen   PlacementMode   IsPopupOpen   ToggleContent   PopupContent d� 	IsChecked �� IsTextSearchEnabled  �� IsTextSearchCaseSensitive �� 
GroupStyle(  "System.Windows.Controls.GroupStyle   HeaderTemplate $  MaterialDesignSubtitle2TextBlock  �� TextAlignment��%  '        �� "   ��  �� $e _pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml?��� $y spack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.LightBlue.xaml?��� $h bpack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml?��� $x rpack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Accent/MaterialDesignColor.LightBlue.xaml?�.���  �@�WindowStyle$
 None=���  � ��$ 2 0 2 2q���  � ��# �   ��  � ��# �  ��  � ��$ 	Tw Cen MT)���  � �� �� "	 �.R�� 
�������.+�]� ��. .+�  $
 	colorZone��$	��Top=�$��35��$ 
PrimaryMid=�$ Depth2=�.���  �� $
��Auto ��� $
��Auto ��� $
��Auto ��� �� $
��Auto ��� $��* ��� $
��Auto ��� $
��Auto �.+�(� $��0��$��8��$��30��$��100,0q�.���� #���  $��Center=�$��Center=�$��Place Fixtures��$
��Bold"�$��5q�
��������� $��7��$ 
Horizontal=�$��Right=�.+��� -    . $��1q�
�������.��  $ WindowMinimize=�
��������� -    . $��1q�
�������.��  $ WindowRestore=�
��������� -    . $��1q�
�������.��  $ WindowClose=�
��������� #��z��#��z��._�� .+�   .  $ MainContent��.���  �� $
��Auto ��� $
��Auto � �� $��75 ��� $��75 ��� $��75 �.+��� $��10q�$��0��$ 
Horizontal=�$��Center=�$��Center=�.+��� $��10q�$��20(�$��Select CAD File��#���  $��Center=��� $��10q�$
 Name��$	��150��$	��Top=����� *ImportedCadFiles+ �� *SelectedCadFile+�� $��10q�$��20(�$��Select Layers��#���  $��Center=�   . $ RightAndAlignTopEdges=�$��10q� �� *IsPopupOpen+   $ SelectSearch=�$��25��$��30��$��Center=�. �� $	��300��$	��250��$
��Auto=�$
��Auto=�.���� ���� *	LayerList+���� Test��.R��� $ 
Horizontal=�.+��� $��5q� �� *	IsChecked+�� $��5q����� *	LayerName+�� $��1��$��Center=�$��2��$��Center=�$ 
Horizontal=�.+��� $��Select Family Type��$��10q�$��20(��� $��10q� .  .  $
 Name��$	��200��$	��Top=����� *	ItemsView+ �� *SelectedFixtureType+    �� .R��� $��10q�#���  $��15(�$��DemiBold"����� *Name+���� .R��� ���� *Name+�� $ 
Horizontal=�$��2��$��2��$��Center=�$��Center=�.+��� $��10q�$	��200��$��Center=�$��Center=����� *CreateFixturesCommand+.���� $ 
Horizontal=�.+��� $��Create Fixtures��$��17(�$  Center=�
��������� $��20,0,0,0q�$	��150��$��Center=����� *CancelCommand+.���� $ 
Horizontal=�.+��� $��Cancel��$��17(�$  Center=�
�������!   WindowChrome!   "   CaptionHeight$" 35Q�#   CornerRadius$# 0��$   ResizeBorderThickness$$ 5q�%   GlassFrameThickness$% 0q�