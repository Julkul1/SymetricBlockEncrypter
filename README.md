# Jan Rogowski Julian Kulikowski Michał Węsiora 

# Dokumentacja do „Implementacja aplikacji szyfrującej i deszyfrującej pliki z wykorzystaniem kryptografii symetrycznej”.

\
## 1.  Instrukcja obsługi aplikacji

Aplikacja stanowi demonstrację mechanizmu szyfrowania i deszyfrowania plików przy pomocy algorytmu blokowego AES w języku C#. Udostępnia ona kilka podstawowych trybów tego algorytmu: ECB, CBC, CFB, CTR. Poprzez szyfrowanie zdjęć pokazuje ona działanie tych trybów, ich cechy i umożliwia zaobserwowanie skutków przekłamań pikseli lub wektora IV dla każdego z kilku obsługiwanych trybów AES.


Przy pomocy przycisku „Select Image” wybieramy zdjęcie do zaszyfrowania.

<img src="media/image1.gif" style="width:6.59173in;height:4.53617in" />


Wybieramy tryb szyfrowania blokowego i po naciśnięciu przycisku „Encrypt” szyfrujemy, a po naciśnięciu przycisku „Decrypt” odszyfrowujemy plik.

<img src="media/image2.gif" style="width:6.53249in;height:4.49541in" />


Aby zapisać zdjęcie na dysku wybieramy przycisk „Save Encrypted Image” albo „Save Decrypted Image”

<img src="media/image3.gif" style="width:6.62577in;height:4.5596in" />


Aplikacja umożliwia również modyfikację wektora IV w celu symulowania przekłamania. Aby dokonać modyfikacji należy najechać na pole tekstowe w lewym dolnym rogu i zmienić wartości szesnastkowe. Zmieniony wektor zostanie automatycznie wykorzystany do odszyfrowania.

<img src="media/image4.gif" style="width:6.42331in;height:4.42027in" />


Możemy również dokonać modyfikacji (przekłamania) wybranego piksela poprzez wybranie pozycji (X, Y) w polach tekstowych, składowych RGB piksela i następnie zatwierdzenie zmian przyciskiem „Submit changes”.

<img src="media/image5.gif" style="width:6.55215in;height:4.50893in" />

\
## 2.  Implementacja AES

Algorytm AES zaimplementowany został w naszej aplikacji przy pomocy
biblioteki System.Security.Cryptography. Poniżej przedstawiamy kawałki
kodu odpowiadające za realizację tego algorytmu.

- Inicjalizacja AES

> <img src="media/image6.png" style="width:3.42167in;height:0.51028in"
> alt="A blue and black text Description automatically generated" />

- Ustawienie klucza szyfrującego

> <img src="media/image7.png" style="width:4.58699in;height:1.30033in"
> alt="A computer code with blue text Description automatically generated" />

- Ustawienie wektora inicjującego IV (dla trybów CBC, CFB, CTR)

> <img src="media/image8.png" style="width:6.3in;height:2.46736in"
> alt="A computer code with text Description automatically generated" />

- Implementacja trybu ECB

> <img src="media/image9.png" style="width:6.3in;height:2.61944in"
> alt="A screen shot of a computer code Description automatically generated" />

- Implementacja trybu CBC

<img src="media/image10.png" style="width:6.3in;height:2.54931in"
alt="A screen shot of a computer code Description automatically generated" />

- Implementacja trybu CFB

<img src="media/image11.png" style="width:6.3in;height:2.55625in" />

- Implementacja trybu CTR

<img src="media/image12.png" style="width:6.3in;height:3.54931in" />

<img src="media/image13.png" style="width:6.3in;height:4.91597in" />
