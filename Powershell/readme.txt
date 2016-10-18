Problém se ètením desetinných èísel v excelu
============================================
Pøi otevøení CSV dokumentu s desetinnými místy se zobrazuje datum místo desetinných èísel

Øešení:
pøenastavit si excel tak, aby bral v potaz desetinnou teèku
pøípadnì pøejmenovat CSV soubor na txt, otevøít v excelu a pomocí "Data" > "Text do sloupce" nastavit støedník a v dalším nastavení upravit místo desetinné èárky na teèku

Odkaz:
http://superuser.com/a/777885
nastavení viz obrázek:
 - excel-decimal-separator.png

Jak dostat CSV do MSSQL
=======================
Db > Import Data > Flat File Source 
https://support.discountasp.net/kb/a1179/how-to-import-a-csv-file-into-a-database-using-sql-server-management-studio.aspx

Instalace manipulace s XLSX
===========================
https://blogs.technet.microsoft.com/heyscriptingguy/2015/11/25/introducing-the-powershell-excel-module-2/
Otevøi powershell jako admin a spust pøíkaz:
Install-Module ImportExcel
