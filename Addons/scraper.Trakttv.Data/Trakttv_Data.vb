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

Imports EmberAPI
Imports NLog

Public Class Trakttv_Data
    Implements Interfaces.ScraperModule_Data_Movie
    Implements Interfaces.ScraperModule_Data_TV


#Region "Fields"
    Shared logger As Logger = LogManager.GetCurrentClassLogger()

    Public Shared _AssemblyName As String
    Public Shared ConfigScrapeOptions_Movie As New Structures.ScrapeOptions
    Public Shared ConfigScrapeOptions_TV As New Structures.ScrapeOptions
    Public Shared ConfigScrapeModifier_Movie As New Structures.ScrapeModifiers
    Public Shared ConfigScrapeModifier_TV As New Structures.ScrapeModifiers

    Private _SpecialSettings_Movie As New SpecialSettings
    Private _SpecialSettings_TV As New SpecialSettings
    Private _Name As String = "Trakttv_Data"
    Private _ScraperEnabled_Movie As Boolean = False
    Private _ScraperEnabled_TV As Boolean = False
    Private _setup_Movie As frmSettingsHolder_Movie
    Private _setup_TV As frmSettingsHolder_TV

#End Region 'Fields

#Region "Events"

    'Movie part
    Public Event ModuleSettingsChanged_Movie() Implements Interfaces.ScraperModule_Data_Movie.ModuleSettingsChanged
    Public Event ScraperEvent_Movie(ByVal eType As Enums.ScraperEventType, ByVal Parameter As Object) Implements Interfaces.ScraperModule_Data_Movie.ScraperEvent
    Public Event ScraperSetupChanged_Movie(ByVal name As String, ByVal State As Boolean, ByVal difforder As Integer) Implements Interfaces.ScraperModule_Data_Movie.ScraperSetupChanged
    Public Event SetupNeedsRestart_Movie() Implements Interfaces.ScraperModule_Data_Movie.SetupNeedsRestart

    'TV part
    Public Event ModuleSettingsChanged_TV() Implements Interfaces.ScraperModule_Data_TV.ModuleSettingsChanged
    Public Event ScraperEvent_TV(ByVal eType As Enums.ScraperEventType, ByVal Parameter As Object) Implements Interfaces.ScraperModule_Data_TV.ScraperEvent
    Public Event ScraperSetupChanged_TV(ByVal name As String, ByVal State As Boolean, ByVal difforder As Integer) Implements Interfaces.ScraperModule_Data_TV.ScraperSetupChanged
    Public Event SetupNeedsRestart_TV() Implements Interfaces.ScraperModule_Data_TV.SetupNeedsRestart

#End Region 'Events

#Region "Properties"

    ReadOnly Property ModuleName() As String Implements Interfaces.ScraperModule_Data_Movie.ModuleName, Interfaces.ScraperModule_Data_TV.ModuleName
        Get
            Return _Name
        End Get
    End Property

    ReadOnly Property ModuleVersion() As String Implements Interfaces.ScraperModule_Data_Movie.ModuleVersion, Interfaces.ScraperModule_Data_TV.ModuleVersion
        Get
            Return FileVersionInfo.GetVersionInfo(Reflection.Assembly.GetExecutingAssembly.Location).FileVersion.ToString
        End Get
    End Property

    Property ScraperEnabled_Movie() As Boolean Implements Interfaces.ScraperModule_Data_Movie.ScraperEnabled
        Get
            Return _ScraperEnabled_Movie
        End Get
        Set(ByVal value As Boolean)
            _ScraperEnabled_Movie = value
        End Set
    End Property

    Property ScraperEnabled_TV() As Boolean Implements Interfaces.ScraperModule_Data_TV.ScraperEnabled
        Get
            Return _ScraperEnabled_TV
        End Get
        Set(ByVal value As Boolean)
            _ScraperEnabled_TV = value
        End Set
    End Property

#End Region 'Properties

#Region "Methods"

    Private Sub Handle_ModuleSettingsChanged_Movie()
        RaiseEvent ModuleSettingsChanged_Movie()
    End Sub

    Private Sub Handle_ModuleSettingsChanged_TV()
        RaiseEvent ModuleSettingsChanged_TV()
    End Sub

    Private Sub Handle_SetupNeedsRestart_Movie()
        RaiseEvent SetupNeedsRestart_Movie()
    End Sub

    Private Sub Handle_SetupNeedsRestart_TV()
        RaiseEvent SetupNeedsRestart_TV()
    End Sub

    Private Sub Handle_SetupScraperChanged_Movie(ByVal state As Boolean, ByVal difforder As Integer)
        ScraperEnabled_Movie = state
        RaiseEvent ScraperSetupChanged_Movie(String.Concat(_Name, "_Movie"), state, difforder)
    End Sub

    Private Sub Handle_SetupScraperChanged_TV(ByVal state As Boolean, ByVal difforder As Integer)
        ScraperEnabled_TV = state
        RaiseEvent ScraperSetupChanged_TV(String.Concat(_Name, "_TV"), state, difforder)
    End Sub

    Sub Init_Movie(ByVal sAssemblyName As String) Implements Interfaces.ScraperModule_Data_Movie.Init
        _AssemblyName = sAssemblyName
        LoadSettings_Movie()
    End Sub

    Sub Init_TV(ByVal sAssemblyName As String) Implements Interfaces.ScraperModule_Data_TV.Init
        _AssemblyName = sAssemblyName
        LoadSettings_TV()
    End Sub

    Function InjectSetupScraper_Movie() As Containers.SettingsPanel Implements Interfaces.ScraperModule_Data_Movie.InjectSetupScraper
        Dim SPanel As New Containers.SettingsPanel
        _setup_Movie = New frmSettingsHolder_Movie
        LoadSettings_Movie()
        _setup_Movie.chkEnabled.Checked = _ScraperEnabled_Movie

        _setup_Movie.chkMainRating.Checked = ConfigScrapeOptions_Movie.bMainRating
        _setup_Movie.chkMainUserRating.Checked = ConfigScrapeOptions_Movie.bMainUserRating

        _setup_Movie.orderChanged()

        SPanel.Name = String.Concat(_Name, "_Movie")
        SPanel.Text = "Trakttv"
        SPanel.Prefix = "TrakttvMovieInfo_"
        SPanel.Order = 110
        SPanel.Parent = "pnlMovieData"
        SPanel.Type = Master.eLang.GetString(36, "Movies")
        SPanel.ImageIndex = If(_ScraperEnabled_Movie, 9, 10)
        SPanel.Panel = _setup_Movie.pnlSettings

        AddHandler _setup_Movie.SetupScraperChanged, AddressOf Handle_SetupScraperChanged_Movie
        AddHandler _setup_Movie.ModuleSettingsChanged, AddressOf Handle_ModuleSettingsChanged_Movie
        Return SPanel
    End Function

    Function InjectSetupScraper_TV() As Containers.SettingsPanel Implements Interfaces.ScraperModule_Data_TV.InjectSetupScraper
        Dim SPanel As New Containers.SettingsPanel
        _setup_TV = New frmSettingsHolder_TV
        LoadSettings_TV()
        _setup_TV.chkEnabled.Checked = _ScraperEnabled_TV

        _setup_TV.chkEpisodeRating.Checked = ConfigScrapeOptions_TV.bEpisodeRating
        _setup_TV.chkMainRating.Checked = ConfigScrapeOptions_TV.bMainRating
        _setup_TV.chkMainRating.Checked = ConfigScrapeOptions_TV.bMainUserRating

        _setup_TV.orderChanged()

        SPanel.Name = String.Concat(_Name, "_TV")
        SPanel.Text = "Trakttv"
        SPanel.Prefix = "TrakttvTVInfo_"
        SPanel.Order = 110
        SPanel.Parent = "pnlTVData"
        SPanel.Type = Master.eLang.GetString(653, "TV Shows")
        SPanel.ImageIndex = If(_ScraperEnabled_TV, 9, 10)
        SPanel.Panel = _setup_TV.pnlSettings

        AddHandler _setup_TV.SetupScraperChanged, AddressOf Handle_SetupScraperChanged_TV
        AddHandler _setup_TV.ModuleSettingsChanged, AddressOf Handle_ModuleSettingsChanged_TV
        Return SPanel
    End Function

    Sub LoadSettings_Movie()
        ConfigScrapeOptions_Movie.bMainRating = AdvancedSettings.GetBooleanSetting("DoRating", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainUserRating = AdvancedSettings.GetBooleanSetting("DoUserRating", True, , Enums.ContentType.Movie)
        _SpecialSettings_Movie.AccessToken = AdvancedSettings.GetSetting("AccessToken", String.Empty)
        _SpecialSettings_Movie.CreatedAt = AdvancedSettings.GetSetting("CreatedAt", "0")
        _SpecialSettings_Movie.ExpiresInSeconds = AdvancedSettings.GetSetting("ExpiresInSeconds", "0")
        _SpecialSettings_Movie.RefreshToken = AdvancedSettings.GetSetting("RefreshToken", String.Empty)
    End Sub

    Sub LoadSettings_TV()
        ConfigScrapeOptions_TV.bEpisodeRating = AdvancedSettings.GetBooleanSetting("DoRating", True, , Enums.ContentType.TVEpisode)
        ConfigScrapeOptions_TV.bEpisodeUserRating = AdvancedSettings.GetBooleanSetting("DoUserRating", True, , Enums.ContentType.TVEpisode)
        ConfigScrapeOptions_TV.bMainRating = AdvancedSettings.GetBooleanSetting("DoRating", True, , Enums.ContentType.TVShow)
        ConfigScrapeOptions_TV.bMainUserRating = AdvancedSettings.GetBooleanSetting("DoUserRating", True, , Enums.ContentType.TVShow)
        _SpecialSettings_TV.AccessToken = AdvancedSettings.GetSetting("AccessToken", String.Empty)
        _SpecialSettings_TV.CreatedAt = AdvancedSettings.GetSetting("CreatedAt", "0")
        _SpecialSettings_TV.ExpiresInSeconds = AdvancedSettings.GetSetting("ExpiresInSeconds", "0")
        _SpecialSettings_TV.RefreshToken = AdvancedSettings.GetSetting("RefreshToken", String.Empty)
    End Sub

    Sub SaveSettings_Movie()
        Using settings = New AdvancedSettings()
            settings.SetBooleanSetting("DoRating", ConfigScrapeOptions_Movie.bMainRating, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoUserRating", ConfigScrapeOptions_Movie.bMainUserRating, , , Enums.ContentType.Movie)
            settings.SetSetting("AccessToken", _SpecialSettings_Movie.AccessToken)
            settings.SetSetting("CreatedAt", _SpecialSettings_Movie.CreatedAt)
            settings.SetSetting("ExpiresInSeconds", _SpecialSettings_Movie.ExpiresInSeconds)
            settings.SetSetting("RefreshToken", _SpecialSettings_Movie.RefreshToken)
        End Using
    End Sub

    Sub SaveSettings_TV()
        Using settings = New AdvancedSettings()
            settings.SetBooleanSetting("DoRating", ConfigScrapeOptions_TV.bEpisodeRating, , , Enums.ContentType.TVEpisode)
            settings.SetBooleanSetting("DoRating", ConfigScrapeOptions_TV.bMainRating, , , Enums.ContentType.TVShow)
            settings.SetBooleanSetting("DoUserRating", ConfigScrapeOptions_TV.bEpisodeUserRating, , , Enums.ContentType.TVEpisode)
            settings.SetBooleanSetting("DoUserRating", ConfigScrapeOptions_TV.bMainUserRating, , , Enums.ContentType.TVShow)
            settings.SetSetting("AccessToken", _SpecialSettings_TV.AccessToken)
            settings.SetSetting("CreatedAt", _SpecialSettings_TV.CreatedAt)
            settings.SetSetting("ExpiresInSeconds", _SpecialSettings_TV.ExpiresInSeconds)
            settings.SetSetting("RefreshToken", _SpecialSettings_TV.RefreshToken)
        End Using
    End Sub

    Sub SaveSetupScraper_Movie(ByVal DoDispose As Boolean) Implements Interfaces.ScraperModule_Data_Movie.SaveSetupScraper
        ConfigScrapeOptions_Movie.bMainRating = _setup_Movie.chkMainRating.Checked
        ConfigScrapeOptions_Movie.bMainUserRating = _setup_Movie.chkMainUserRating.Checked

        SaveSettings_Movie()
        If DoDispose Then
            RemoveHandler _setup_Movie.SetupScraperChanged, AddressOf Handle_SetupScraperChanged_Movie
            RemoveHandler _setup_Movie.ModuleSettingsChanged, AddressOf Handle_ModuleSettingsChanged_Movie
            _setup_Movie.Dispose()
        End If
    End Sub

    Sub SaveSetupScraper_TV(ByVal DoDispose As Boolean) Implements Interfaces.ScraperModule_Data_TV.SaveSetupScraper
        ConfigScrapeOptions_TV.bEpisodeRating = _setup_TV.chkEpisodeRating.Checked
        ConfigScrapeOptions_TV.bEpisodeUserRating = _setup_TV.chkEpisodeUserRating.Checked
        ConfigScrapeOptions_TV.bMainRating = _setup_TV.chkMainRating.Checked
        ConfigScrapeOptions_TV.bMainUserRating = _setup_TV.chkMainUserRating.Checked

        SaveSettings_TV()
        If DoDispose Then
            RemoveHandler _setup_TV.SetupScraperChanged, AddressOf Handle_SetupScraperChanged_TV
            RemoveHandler _setup_TV.ModuleSettingsChanged, AddressOf Handle_ModuleSettingsChanged_TV
            _setup_TV.Dispose()
        End If
    End Sub


    ''' <summary>
    '''  Scrape MovieDetails from Trakttv
    ''' </summary>
    ''' <param name="oDBMovie">Movie to be scraped. oDBMovie as ByRef to use existing data for identifing movie and to fill with IMDB/TMDB ID for next scraper</param>
    ''' <param name="nMovie">New scraped movie data</param>
    ''' <param name="Options">What kind of data is being requested from the scrape(global scraper settings)</param>
    ''' <returns>Database.DBElement Object (nMovie) which contains the scraped data</returns>
    ''' <remarks></remarks>
    Function Scraper_Movie(ByRef oDBElement As Database.DBElement, ByRef ScrapeModifiers As Structures.ScrapeModifiers, ByRef ScrapeType As Enums.ScrapeType, ByRef ScrapeOptions As Structures.ScrapeOptions) As Interfaces.ModuleResult_Data_Movie Implements Interfaces.ScraperModule_Data_Movie.Scraper_Movie
        logger.Trace("[Tracktv_Data] [Scraper_Movie] [Start]")

        Dim nMovie As MediaContainers.Movie = Nothing

        If ScrapeModifiers.MainNFO Then
            LoadSettings_Movie()

            Dim _scraper As New TrakttvScraper.Scraper(_SpecialSettings_Movie)
            If _scraper.NewTokenCreated Then
                SaveSettings_Movie()
            End If

            Dim FilteredOptions As Structures.ScrapeOptions = Functions.ScrapeOptionsAndAlso(ScrapeOptions, ConfigScrapeOptions_Movie)

            Dim mDBElement As Database.DBElement = oDBElement
            Dim Result = _scraper.GetInfo_Movie(_scraper.GetTraktID(mDBElement), FilteredOptions)
            While Not Result.IsCompleted
                Threading.Thread.Sleep(50)
            End While
            If Result.Exception Is Nothing AndAlso Result.Result IsNot Nothing Then
                nMovie = Result.Result
            ElseIf Result.Exception IsNot Nothing Then
                logger.Error(String.Concat("[Tracktv_Data] [Scraper_Movie]: ", Result.Exception.InnerException.Message))
            End If
        End If

        logger.Trace("[Tracktv_Data] [Scraper_Movie] [Done]")
        Return New Interfaces.ModuleResult_Data_Movie With {.Result = nMovie}
    End Function

    ''' <summary>
    '''  Scrape tv show details from Trakttv
    ''' </summary>
    ''' <param name="oDBElement">TV Show to be scraped. DBTV as ByRef to use existing data for identifing tv show and to fill with IMDB/TMDB/TVDB ID for next scraper</param>
    ''' <param name="ScrapeModifiers">what additional data to scrape (images, episode details, season details...)</param>
    ''' <param name="ScrapeOptions">What kind of data is being requested from the scrape(global scraper settings)</param>
    ''' <returns>modifies Database.DBElement Object which contains the scraped data</returns>
    ''' <remarks></remarks>
    Function Scraper_TV(ByRef oDBElement As Database.DBElement, ByRef ScrapeModifiers As Structures.ScrapeModifiers, ByRef ScrapeType As Enums.ScrapeType, ByRef ScrapeOptions As Structures.ScrapeOptions) As Interfaces.ModuleResult_Data_TVShow Implements Interfaces.ScraperModule_Data_TV.Scraper_TVShow
        logger.Trace("[Tracktv_Data] [Scraper_TV] [Start]")

        Dim nTVShow As MediaContainers.TVShow = Nothing

        If ScrapeModifiers.MainNFO Then
            LoadSettings_TV()

            Dim _scraper As New TrakttvScraper.Scraper(_SpecialSettings_TV)
            If _scraper.NewTokenCreated Then
                SaveSettings_TV()
            End If

            Dim FilteredOptions As Structures.ScrapeOptions = Functions.ScrapeOptionsAndAlso(ScrapeOptions, ConfigScrapeOptions_TV)

            Dim mDBElement As Database.DBElement = oDBElement
            Dim mScrapeModifiers As Structures.ScrapeModifiers = ScrapeModifiers
            Dim Result = _scraper.GetInfo_TVShow(_scraper.GetTraktID(mDBElement), mScrapeModifiers, FilteredOptions, mDBElement.Episodes)
            While Not Result.IsCompleted
                Threading.Thread.Sleep(50)
            End While
            If Result.Exception Is Nothing AndAlso Result.Result IsNot Nothing Then
                nTVShow = Result.Result
            ElseIf Result.Exception IsNot Nothing Then
                logger.Error(String.Concat("[Tracktv_Data] [Scraper_TV]: ", Result.Exception.InnerException.Message))
            End If
        End If

        logger.Trace("[Tracktv_Data] [Scraper_TV] [Done]")
        Return New Interfaces.ModuleResult_Data_TVShow With {.Result = nTVShow}
    End Function

    ''' <summary>
    '''  Scrape episode details from Trakttv
    ''' </summary>
    ''' <param name="oDBElement">TV Season to be scraped as ByRef to use existing data for identifing tv show and to fill with IMDB/TMDB/TVDB ID for next scraper</param>
    ''' <param name="ScrapeOptions">What kind of data is being requested from the scrape(global scraper settings)</param>
    ''' <returns>modifies Database.DBElement Object which contains the scraped data</returns>
    ''' <remarks></remarks>
    Public Function Scraper_TVEpisode(ByRef oDBElement As Database.DBElement, ByVal ScrapeOptions As Structures.ScrapeOptions) As Interfaces.ModuleResult_Data_TVEpisode Implements Interfaces.ScraperModule_Data_TV.Scraper_TVEpisode
        logger.Trace("[Tracktv_Data] [Scraper_TVEpisode] [Start]")

        Dim nTVEpisode As MediaContainers.EpisodeDetails = Nothing

        LoadSettings_TV()

        Dim _scraper As New TrakttvScraper.Scraper(_SpecialSettings_TV)
        If _scraper.NewTokenCreated Then
            SaveSettings_TV()
        End If

        Dim FilteredOptions As Structures.ScrapeOptions = Functions.ScrapeOptionsAndAlso(ScrapeOptions, ConfigScrapeOptions_TV)

        Dim mDBElement As Database.DBElement = oDBElement
        Dim Result = _scraper.GetInfo_TVEpisode(_scraper.GetTraktID(mDBElement, True), mDBElement.TVEpisode.Season, mDBElement.TVEpisode.Episode, FilteredOptions)
        While Not Result.IsCompleted
            Threading.Thread.Sleep(50)
        End While
        If Result.Exception Is Nothing AndAlso Result.Result IsNot Nothing Then
            nTVEpisode = Result.Result
        ElseIf Result.Exception IsNot Nothing Then
            logger.Error(String.Concat("[Tracktv_Data] [Scraper_TVEpisode]: ", Result.Exception.InnerException.Message))
        End If

        logger.Trace("[Tracktv_Data] [Scraper_TVEpisode] [Done]")
        Return New Interfaces.ModuleResult_Data_TVEpisode With {.Result = nTVEpisode}
    End Function

    ''' <summary>
    '''  Scrape season details from Trakttv
    ''' </summary>
    ''' <param name="oDBElement">TV Season to be scraped as ByRef to use existing data for identifing tv show and to fill with IMDB/TMDB/TVDB ID for next scraper</param>
    ''' <param name="ScrapeOptions">What kind of data is being requested from the scrape(global scraper settings)</param>
    ''' <returns>Database.DBElement Object (nMovie) which contains the scraped data</returns>
    ''' <remarks></remarks>
    Public Function Scraper_TVSeason(ByRef oDBElement As Database.DBElement, ByVal ScrapeOptions As Structures.ScrapeOptions) As Interfaces.ModuleResult_Data_TVSeason Implements Interfaces.ScraperModule_Data_TV.Scraper_TVSeason
        Return New Interfaces.ModuleResult_Data_TVSeason With {.Result = Nothing}
    End Function

    Function GetMovieStudio(ByRef DBMovie As Database.DBElement, ByRef studio As List(Of String)) As Interfaces.ModuleResult Implements Interfaces.ScraperModule_Data_Movie.GetMovieStudio
        If (DBMovie.Movie Is Nothing OrElse String.IsNullOrEmpty(DBMovie.Movie.IMDB)) Then
            logger.Error("Attempting to get studio for undefined movie")
            Return New Interfaces.ModuleResult
        End If

        LoadSettings_Movie()
        Return New Interfaces.ModuleResult With {.breakChain = False}
    End Function

    Function GetTMDBID(ByVal sIMDBID As String, ByRef sTMDBID As String) As Interfaces.ModuleResult Implements Interfaces.ScraperModule_Data_Movie.GetTMDBID
        Return New Interfaces.ModuleResult With {.breakChain = False}
    End Function

    Public Sub ScraperOrderChanged_Movie() Implements Interfaces.ScraperModule_Data_Movie.ScraperOrderChanged
        _setup_Movie.orderChanged()
    End Sub

    Public Sub ScraperOrderChanged_TV() Implements Interfaces.ScraperModule_Data_TV.ScraperOrderChanged
        _setup_TV.orderChanged()
    End Sub

#End Region 'Methods

#Region "Nested Types"

    Structure SpecialSettings

#Region "Fields"

        Dim AccessToken As String
        Dim CreatedAt As String
        Dim ExpiresInSeconds As String
        Dim RefreshToken As String

#End Region 'Fields

    End Structure

#End Region 'Nested Types

End Class