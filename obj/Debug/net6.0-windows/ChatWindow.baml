   M S B A M L   `   `   `  ����  LocalMessenger    LocalMessenger.ChatWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Чат��5   	   $	��450��6   $	��800��6"   .��5      �  � ColumnDefinitions 5   
   �� 5      $	��200 �6   �� 5      $��* �6    � RowDefinitions 5
   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��Пользователи:��6   $��0��6(   $��0��68   $��5q�6E   $
��Bold"�6P   �� 5   
   -   5         ��Name$ lstUsers��5      $��0��6$   $��0��64   $��5,25,5,5q�6A   �� 5   
   -   5         ��Name$ txtChat��5      $��1��6#   $��0��63   $��5q�6@    �� 
IsReadOnly$
 True��6K    �� TextWrapping$
 Wrap=�6]   " �� VerticalScrollBarVisibility$
 Auto=�6q   � 5   
   $��1��6   $��1��6   $��5q�6,    5      �� 5      $��* �6#   �� 5      $
��Auto �6#   �� 5      $
��Auto �6#   .+�5      �� -   5        $ 
txtMessage��5      $��0��6*   $��5,0,84,0q�6:   �� 5      -   5        $��Отправить��5      $��	511,0,0,0q�6)   $��17��6O   $��Bottom=�6[   �� 5      -   5        $��Отключиться��5      $��-192,0,607,0q�6D   5      5      5      