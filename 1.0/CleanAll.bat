@ECHO OFF
pushd "%~dp0"
ECHO.
ECHO.
ECHO.
ECHO This script deletes all temporary build files in their
ECHO corresponding BIN and OBJ Folder contained in the following projects
ECHO.
ECHO WPFApp\Locult
ECHO WPFApp\LocultApp
ECHO.
ECHO WPFMetroApp\LocultMetro
ECHO.
ECHO Services\ResourceFileLib
ECHO Services\AppResourcesLib
ECHO.
ECHO Services\ServiceLocatorInterfaces
ECHO Services\ExplorerLib
ECHO.
ECHO Services\MsgBox\MsgBox
ECHO Services\MsgBox\UserNotification
ECHO.
ECHO Services\MSTranslator
ECHO Services\Processing
ECHO Services\ServiceLocator
ECHO Services\Settings
ECHO Services\SettingsModel
ECHO.
ECHO TranslationSolutionViewModelLib
ECHO.
ECHO TranslatorSolution\TranslatorSolutionModel
ECHO TranslatorSolution\TranslatorSolutionUnitTests
ECHO TranslatorSolution\TranslatorSolutionXML
ECHO.
REM Ask the user if hes really sure to continue beyond this point XXXXXXXX
set /p choice=Are you sure to continue (Y/N)?
if not '%choice%'=='Y' Goto EndOfBatch
REM Script does not continue unless user types 'Y' in upper case letter
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

ECHO.
ECHO Deleting BIN and OBJ Folders in WPFApp\Locult project folder
ECHO.
RMDIR /S /Q WPFApp\Locult\bin
RMDIR /S /Q WPFApp\Locult\obj
ECHO.

ECHO Deleting BIN and OBJ Folders in WPFApp\LocultApp project folder
ECHO.
RMDIR /S /Q WPFApp\LocultApp\bin
RMDIR /S /Q WPFApp\LocultApp\obj
ECHO.

ECHO.
ECHO Deleting BIN and OBJ Folders in WPFMetroApp\LocultMetro project folder
ECHO.
RMDIR /S /Q WPFMetroApp\LocultMetro\bin
RMDIR /S /Q WPFMetroApp\LocultMetro\obj
ECHO.

ECHO.
ECHO Deleting BIN and OBJ Folders in Services\ResourceFileLib project folder
ECHO.
RMDIR /S /Q Services\ResourceFileLib\bin
RMDIR /S /Q Services\ResourceFileLib\obj
ECHO.
ECHO Deleting BIN and OBJ Folders in Services\AppResourcesLib project folder
ECHO.
RMDIR /S /Q Services\AppResourcesLib\bin
RMDIR /S /Q Services\AppResourcesLib\obj

ECHO.
ECHO Deleting BIN and OBJ Folders in Services\Processing project folder
ECHO.
RMDIR /S /Q Services\Processing\bin
RMDIR /S /Q Services\Processing\obj
ECHO.
ECHO Deleting BIN and OBJ Folders in Services\ServiceLocatorInterfaces project folder
ECHO.
RMDIR /S /Q Services\ServiceLocatorInterfaces\bin
RMDIR /S /Q Services\ServiceLocatorInterfaces\obj

ECHO.
ECHO Deleting BIN and OBJ Folders in Services\ExplorerLib project folder
ECHO.
RMDIR /S /Q Services\ExplorerLib\bin
RMDIR /S /Q Services\ExplorerLib\obj

ECHO.
ECHO Deleting BIN and OBJ Folders in Services\MsgBox\MsgBox project folder
ECHO.
RMDIR /S /Q Services\MsgBox\MsgBox\bin
RMDIR /S /Q Services\MsgBox\MsgBox\obj

ECHO.
ECHO Deleting BIN and OBJ Folders in Services\MsgBox\UserNotification project folder
ECHO.
RMDIR /S /Q Services\MsgBox\UserNotification\bin
RMDIR /S /Q Services\MsgBox\UserNotification\obj

ECHO.
ECHO Deleting BIN and OBJ Folders in Services\MSTranslator project folder
ECHO.
RMDIR /S /Q Services\MSTranslator\bin
RMDIR /S /Q Services\MSTranslator\obj

ECHO.
ECHO Deleting BIN and OBJ Folders in Services\ServiceLocator project folder
ECHO.
RMDIR /S /Q Services\ServiceLocator\bin
RMDIR /S /Q Services\ServiceLocator\obj

ECHO.
ECHO Deleting BIN and OBJ Folders in Services\Settings project folder
ECHO.
RMDIR /S /Q Services\Settings\bin
RMDIR /S /Q Services\Settings\obj

ECHO.
ECHO Deleting BIN and OBJ Folders in Services\Settings project folder
ECHO.
RMDIR /S /Q Services\SettingsModel\bin
RMDIR /S /Q Services\SettingsModel\obj

ECHO.
ECHO Deleting BIN and OBJ Folders in TranslationSolutionViewModelLib project folder
ECHO.
RMDIR /S /Q TranslationSolutionViewModelLib\bin
RMDIR /S /Q TranslationSolutionViewModelLib\obj

ECHO.
ECHO Deleting BIN and OBJ Folders in TranslatorSolution\TranslatorSolutionModel project folder
ECHO.
RMDIR /S /Q TranslatorSolution\TranslatorSolutionModel\bin
RMDIR /S /Q TranslatorSolution\TranslatorSolutionModel\obj

ECHO.
ECHO Deleting BIN and OBJ Folders in TranslatorSolution\TranslatorSolutionUnitTests project folder
ECHO.
RMDIR /S /Q TranslatorSolution\TranslatorSolutionUnitTests\bin
RMDIR /S /Q TranslatorSolution\TranslatorSolutionUnitTests\obj

ECHO.
ECHO Deleting BIN and OBJ Folders in TranslatorSolution\TranslatorSolutionXML project folder
ECHO.
RMDIR /S /Q TranslatorSolution\TranslatorSolutionXML\bin
RMDIR /S /Q TranslatorSolution\TranslatorSolutionXML\obj
