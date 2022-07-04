devtools::load_all('~/git_Projects/cellexalvrR')
cellexalObj@outpath = getwd()

cellexalObj = sessionPath(cellexalObj, 'API_Implemntation')

initWebAPI( cellexalObj, 8001)
