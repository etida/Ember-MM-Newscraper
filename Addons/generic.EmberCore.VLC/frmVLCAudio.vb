﻿' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################
' ################################################################################
' # This file is part of Ember Media Manager.                                    #
' #                                                                              #
' # Ember Media Manager is free software: you can redistribute it and/or modify  #
' # it under the terms of the GNU General Public License as published by         #
' # the Free Software Foundation, either version 3 of the License, or            #
' # (at your option) any later version.                                          #
' #                                                                              #
' # Ember Media Manager is distributed in the hope that it will be useful,       #
' # but WITHOUT ANY WARRANTY; without even the implied warranty of               #
' # MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the                #
' # GNU General Public License for more details.                                 #
' #                                                                              #
' # You should have received a copy of the GNU General Public License            #
' # along with Ember Media Manager.  If not, see <http://www.gnu.org/licenses/>. #
' ################################################################################

Imports System.Windows.Forms
Imports System.IO
Imports System.Text.RegularExpressions
Imports EmberAPI
Imports NLog

Public Class frmVLCAudio

#Region "Fields"

    Shared logger As Logger = NLog.LogManager.GetCurrentClassLogger()
    Private PreviousFrameValue As Integer

#End Region

#Region "Events"

    Event GenericEvent(ByVal mType As EmberAPI.Enums.ModuleEventType, ByRef _params As System.Collections.Generic.List(Of Object))

#End Region

#Region "Methods"

    Sub SetUp()
        Me.lblExtractingFrame.Text = Master.eLang.GetString(306, "Extracting Frame...")
        Me.btnFrameLoad.Text = Master.eLang.GetString(308, "Load Episode")
        Me.btnFrameSave.Text = Master.eLang.GetString(309, "Save as Poster")
    End Sub

#End Region

End Class
