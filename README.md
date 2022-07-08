# DataInterfaceCellexalVR

to see how this does work you should install the latest cellexalvrR webAPI branch:

```
devtools::install_github("sonejilab/cellexalvrR", ref="webapi" )
```

Afterwards you should start the R server by
```
cd testServer 
Rscript startTheServer.R
```

You of cause would also need the Microsoft dotnet - I use Ubuntu 22.04 and the dotnet with the 6.0 library. I also have some additions installed:


```
dotnet add package Newtonsoft.Json
dotnet add package Microsoft.AspNet.Razor
```

In the github path you can test the functionallity of the interface with


```
dotnet build
bin/Debug/net6.0/DataInterfaceCellexalVR Dntt
```


This will write out the following lines:

```
1529: HSPC_001 2.45111933485539
DRC names: diffusion DDRtree tSNE
DRC diffusion(n=1654): HSPC_001 -0.0147601096045195 -0.024119177234672 -0.00799829263520753
DRC DDRtree(n=1654): HSPC_001 194.611148585119 -43.4296139320622 27.2057310634995
DRC tSNE(n=1654): HSPC_001 24.3528197257833 12.8947640732543 3.84250775141795
use this grouping: Prog_840 #FF7D00 DDRtree 3
differential genes :["Pigq","Bag2","Smim13","Tnfaip2","Minpp1","B3galnt2","Ppp2r1b","Pla2g12a","Asns","Gata1","Slc26a1","Epdr1","Scrn3","Mfsd2b","Narf","Spryd4","Xk","Gpsm2","Tlk1","Tango2","Fech","Rnf139","Casp3","Rfesd","Picalm","Ssx2ip","Hba-a2","Atp7b","Slc25a21","Nxpe2","Aldh1a7","Tspan8","Lmna","Slc41a3","Car1","Cldn13","Camsap2","Gypa","Slc40a1","Cela1","Abcb4","Tspo2","Atp1b2","Pklr","Sh3tc2","Klf1","Spon2","Net1","Tmem56","Btnl10","Sun1","Spire1","Hpn","Mt2","Cd55","Nfia","Ampd3","Gstm5","Ces2g","Ell2","Slc14a1","Clca3a1","Myo1d","Wdsub1","Aldh1a1","Cenpk","Birc5","Asf1b","Ifi30","Pik3r2","Mns1","Wdr55","Mt1","Sppl2b","Ftsj1","Phf10","Hagh","Itga5","Golm1","Lrrc8c","Steap3","Gfm1","Psmd9","Alad","Fcgr2b","Csf1r","I830077J02Rik","Epx","Cst7","Papss2","Slpi","Ltb4r1","Ly86","Igsf6","Cldn15","Fam64a","Abcd2","Rab44","Mcpt8","Hp","Hdc","Fcgr3","Ms4a3","Anxa3","Hpse","Atp8b4","Clec12a","Lgals1","Atp1a3","Tifab","Ly6c2","Ly6g","Ccl9","Tyrobp","Mpeg1","Cd68","Pyhin1","F630028O10Rik","Irf8","Sorl1","Mgat5","Tmem229b","Ctsg","Cpa3","Elane","Mpo","F13a1","Cd48","Siglecf","Cep55","Gm20342","Cd93","Emb","Fcer1g","Rin3","Fes","St8sia4","Igfbp4","Prtn3","Nfam1","Cd53","Parp8","Sh2d5","Emilin1","Tm6sf1","Tcirg1","Tspan2","Adgrg3","Cd69","Flnb","Ankrd44","Lmo4","Ms4a6c","Bcl2","Gpc1","Itgal","Pkib","Nkg7","Prss57","Arl11","Eml4","Milr1","Rassf4","Spns3","BC035044","Sell","Plac8","Hk3","Ifi203","Pik3ip1","Ifitm1","Angpt1","Lsp1","Ctla2a","Zbtb20","Mfng","Ebi3","Myct1","Gcnt2","Tbxas1","Pear1","Gimap1","Mir99ahg","Tsc22d3","Cyp2j9","Lpp","Dennd2d","Camk2d","Ldhb","Padi4","Meis1","Neurl3","Gm4759","Ctla2b","Cers4","Rnf122","Maml2","Gimap6","Ptgs1","Myl10","Rgs1","Pygm","Mpl","Tnip3","Plxna4os1","Mecom","Chrnb1","Eya2","Trim30b","Npdc1","Trpc6","Ccl4","Lax1","Ly6a","Casp12","Pglyrp2","Hlf","Scarf1","Fstl1","Csad","Maged1","Sdc4","Tmem37","Gprc5b","Cd74","Sla2","Zfp467","Gimap1os","Hacd4","Tle2","Rtp4","Socs2","Gimap3","Gimap5","Tgtp1","Gucy1a3","Plxdc2","Ltb","Oasl2","Ddx58","Robo4","Cdh15","Iigp1","Mmrn1","Gm4951","Ifi44","Fgd5","Mycn","Gimap8","Cdkn1c","Esam","F11r"]
as expected - server is down.
```

There is more output from the script now that I added more to that package.
Check it out yourself!

This interface seams to be pretty complete to me.


# Outlook

This example here is not fit for CellexalVR as the web lib used here can not yeald which would render the VR unresponsive during a request. And this is of cause not an option. To actually integrate it into CellexalVR I need to use the [Unity network class](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.html).

Probably like that [https://stackoverflow.com/questions/43595004/yielding-on-unitywebrequests-send-makes-two-calls-instead-of-just-one)(stackoverlow post).

```
//UNITY C# REQUEST
IEnumerator RequestRoutine () {
    using ( UnityWebRequest req = UnityWebRequest.Get ( "http://localhost:8080/blablabla" ) ) {
        yield return req.Send ();

        yield return req.isDone;
        Debug.Log ( "is done" );
    }
}

//NODE.JS server
app.get('/blablabla', (req, res) => {
    console.log('request received')
    res.end()
})
```

But for that to be possible I need a Unity project where I can add my code. And CellexalVR does not even load on Linux. It is stuck in "Initial Asset Database Refresh". Whichever unity version I try...