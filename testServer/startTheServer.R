devtools::load_all('~/git/cellexalvrR')
cellexalObj@outpath = getwd()

if ( dir.exists( 'API_Implemntation' )){
	unlink("API_Implemntation", recursive=TRUE)
}
unlink( "selection*.txt" )
cellexalObj = reset( cellexalObj )
cellexalObj = sessionPath(cellexalObj, 'API_Implemntation')

initWebAPI( cellexalObj, 8001)
