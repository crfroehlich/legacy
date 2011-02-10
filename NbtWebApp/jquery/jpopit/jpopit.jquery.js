var __init = false,
	  __count = 0;
$.jpopit = function(message, params) {
	__count++;

	(!params) ? params={} : function(){/* params is defined so don't create a new object */};
	params.fadeInTime = params.fadeInTime || 500;
	params.fadeOutTime = params.fadeOutTime || 2000;
	params.delay = params.delay || 5000;
	params.static = params.static || false;
	params.iconSrc = params.iconSrc || "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADIAAAAyCAYAAAAeP4ixAAAACXBIWXMAAAsTAAALEwEAmpwYAAAABGdBTUEAALGOfPtRkwAAACBjSFJNAAB6JQAAgIMAAPn%2FAACA6QAAdTAAAOpgAAA6mAAAF2%2BSX8VGAAARhElEQVR42mJkoBwwAjETFsyIhP8j4X9Y8H9KHPD%2F%2F38GgABiodDxLJt3H9bg5%2Bc15uPi1OHlYlNhY2OVZ2VhEWNiYhIAYvZ%2F%2F%2F79%2FPP778eff%2F68%2BvLt5%2BP3X3%2Fc%2B%2Fjh87VnD25cTE9JvA404xcQ%2F4Fisj0FEECMJKoHOZ5558FT2hKi%2FB4CPFwOnBzsFiwsLPzMzMwMTIxMYC%2F%2B%2Fw8xmRFqBcxl4Oj494%2Fh%2F7%2F%2FDD9%2B%2Ff784u3HM0%2Bfvz7y5M7lPZnpqZeB0j%2BB%2BDcQ%2F4V6iugYAQggRhI8zHzgxCVrSWHuUH5ebh82VjZ5RmYmBmCoMzAzMTMwswAxMwuYz8iIcDwD1FP%2FgJb9%2FfMXjP%2F9%2B8vwB%2BohEHz36dvTW%2Fef7X5258rGtJT4I0Ad36Ax9ZeYGAJ5BCCAiPEI0%2BY9R1WUZESTBPl5o9jZ2WVBDmdihjgeFAvAGGFgYWZC0fTg8QuGDx%2B%2FMBjoqKBaCsTffwE98%2Fcfw1%2Bgh%2F7%2BBWGIp15%2F%2FPL88rU7Gy8c2724s70FlOy%2BQz30j5BHAAKIkVAsHD17OVBKiC%2BTm5vLkRnsYBYGZlYWsOPZWTGz2IFjlxj6Zq5j2LznPNAEZgZZKVGGxuJwhsRwJwy13379Z%2Fjz9w84ln7%2FgXjqNxBfu%2FPo1K1L5xbmZyWtAyr7DE1yOGMH5BGAAGLE54nTF67miQjyFHBwcMqysbExAJMT2BOcbKxYNf0ChpuxWxHDlbvvgcUAHzAugR79CwzUP98YbhzsYVCRF2FgxqLvM9BDv3%2F9Yfjz5zfDr1%2B%2FgZ77y%2FDk1ceXJ0%2BcXFCSETkNqOQDNLlh9QzIIwABxITLE2cuXa8WFeJrZmVll2VhYWVgAXqCjZMTwxMgk4EpheEHsMzZf%2Bw6w5V7nxgY2MUYGPg0GRh4VBkYeNUZGDjEGSYuPAzM4AwMX4FZ%2BQeaU3jZGBn4eViB%2FoZgRmBylRDmE7e1sylon7K0AqhEGIi5Qe7CVUABBBATNk%2BcPH%2BtTJifp4qJmZWLFRgTIE9wcnEysCGp%2FvkP4nhgQAIzLigzA0MGVGoxA%2B1j5oSaxsjAxs7FwCUoz%2FDhOyswTwDVAj39E%2Bihjz%2BAMfEbUkQxQF0oguQZUKEhwMPJbmNvn1LZMrUY6hkuXJ4BCCB0jzAdPHkxTpCfq4iJiYWNlRXkCXagJ7gYWBkRHvj2G%2BIgkMP%2BQT3xF4itTdUZpGWkgFmDg8HMWJehvSWdoaAgleH3P2aGijQ7eJkKVg9kAFMRwydggvnwC5FeJPhAHmFjYALaDfQNgxAfO6u1s1tsZkFdHFBaCIg5sLibASCAkJMs49YDp00lhHkb2dk5lFnZ2BnY2ECe4GbgZIGWNr8hDoDXC%2F9Rq2wQX19ThuHCzbfANP6N4dNnZobzp08w5EZqMzhZyIPlwTH3H1Hz%2FYd66jvQl0zAVAtKuJwczAxff0EC6Q9QkouDlf0HM780F8v%2F%2B9evXHiGVNeAQUNDAwNAACEXOyx83OxJrKysRozATMoMjF42dnYGLlaIDlD6%2FvsftRiFVXz%2FkWo8S2MFhsNLFRievf7G8P4bE4OmnCZYCuyBfwi9YBraeAHzgUn0wxegHTzArAXk8vGxMbx994%2BBkekv0Io%2FDNoaiqoXpbXDgFIXoaXYHwZEymQACCBYFDFt2nvak4%2BbMxJUZDIBi1YmYB3BDYwKUCH%2BFajtDywk0fE%2FTBpY6DCIC3ExaMhwQPiwJAgNRjBG0gOS%2FwulPwIL249AeQFgWgEFJiOwzvoPTEk87CwMZo4uLt5B8Z4gfwIxG3ISAwggWIww83CyBAE18TKCa2cWBlZWDrCXf%2F6EZGZ40w8pKTEghTKKODobyvj3H73YRIshKOMT0DNMvMCY4WNm%2BPYD6COgZ4BtAQZZcSHOP0z8LkAle4D4C3JlCRBAII8wLt9yUIuHm9MdVOqAPMEE1MjKzsTwE1Sq%2FEXKD%2F%2BRkgKSYx8%2FfcVw9MQFIP0ayGeGt6%2BePHvN8OjJK7Da8EAnhtBAB4QZjAgzGRmRPPkfEvsfvwKLZW6QH4Bpm%2BkPUPg3Azcw75g4uZu8enhJ7%2FzpA%2B9BiQWaxP4DBBDII8BWBost0CCJ%2F6BgB5YUoOIPFNW%2F%2FyAs%2B48UI%2F%2BQogAktHzDCYb%2B2TuAeoGxzSoIFARphIYAqEL895vB0vIzOOnAAwKpoGBAK0BAHFCeBBZeDOzA5P39OzBggA74DTRAVk5WkEtA0gSo6iy0ogTX%2BgABBPYIM%2BM%2F499AV7Ox%2FweGDCMDKLODi9e%2FSJkayTP%2FGFCTWai%2FHcM%2FFn6G1dsvMzx7C4xGJk6koh6o4vszhn%2BM7BCPYElKKEkVRgPxtx8QU0AxBCq9%2Fvz5A4wVFmD9xaIBrSDZofnkL0AAgZvlbGwsGr%2FB7RygYmDUgOwDewSWoRkQmfHfP9RMCmJLSggwFCTbM2ybn8IgyQ8S%2FAWvEEFOYWTlBgYCC9wcWL2DjMFiDKgFCaieAbUa%2FgE5f4E%2BA7mRjYURmOQZZaGVIxusCgEIIHBvjoWZVf4v0LS%2FIE%2BANP1DVHjIGOaAf%2F9QHQTzFDc3B0Owpw6Q8w2jwfAf2eH%2FUB0MK7XgAcTAAC%2Ft%2FkIDFBQjIDcyMzMC%2BUyi0NhghZVcAAEETlpMzEzCf5GLQWhc%2F%2FuPlLQY0DL6f0SFiJx3%2Fv%2F7A84T6ABk1p9%2FmMUacmXKgJb3QJ4Bpxto4IIwqB3GyMjMixQbYI8ABBC41PoNdDkL83%2BoBgiGlSrIeeM%2FtnSN5oj%2F%2F%2F9jbW2DRf9hzx%2FINHLeY%2FyPqPn%2FQ2PsN6hDBtHMjDQ2wAAQQOB65Ov3X%2B94uDgkwMnqLwSDlPyD1R%2F%2FMDPkf6TeH6yiZMDTA4IlLUYsrQKYo%2F8xomZ4RqiBoO7xXyAGegFYOYN6mH%2B%2BoJsPEEAgj%2Fz%2F%2FO3XI042NolffyEdnF9%2FQJmKBew4RhzR%2Fw8teaDUNTg88h8puYI9gRxD%2FyBlw3%2BkpMgMzvB%2FGH7%2FhnS4%2FgLT5ucvv4H0zzdI%2FXqwFoAAdJe7DsAgCEX9%2F1%2Fs3q1T1QAKemlqojUdmUh4nJzrq7GY%2BShIagy%2FJhFgj4Kfm32ostS2q4q2%2F4DdBrHe7qoTAW0l4xBT%2FyliAqUK1B%2BhC%2Fi97hSq5HPK9M9MuwACewRYj1wAxcSPHz%2BBlc8PIP4O7Kn9QZRW%2F5BKFhylzV%2Bkli2uGEF3OKxJD6f%2FIbHBsfGX4dv3n0DP%2FGT4CXTbH2DsPH74nOHX94%2F3kUZcwB4BCCBQ0vrL8%2Bvl8RfvmF%2BJCHCJMQNjgwXYfGf59o2Bm4cXWDkyYa0EkTMwtvYTRqkFa3f%2FR00%2BDGgVIXzUBdS0%2F%2FYd2AX%2BBfTED2DA%2FmJ4%2BeEHw771az9%2B%2B%2FD0GtpICwNAAIFjJDkp%2Ftrnb98P%2FAHGynegpm9AA75%2B%2BQoMiV%2BQzI9eMcLwX6TYgsUejqSFniz%2FIuUZ5DrpD7SY%2FgmMhW%2FfvwHd8x2cUkB55NnbLwx%2Ffry78uHV3TvQdtZPWIwABBALlPFTjO3X1jeffngL8QKrfsav4BbnP2Du4xcQYgB1d8HtsP9IpQ1aIfsfpXhmxJm0cBUOyLH8G5gfvnz%2BxPATmCq%2BAfFPaGyc3L3vx59vr04zQFr6X5A9AhBATDC9mQkh2z99%2Brz5O7DJ%2B%2B3bV2Dp8AkYK18Y3r9%2FD4yZ39BmAjRW%2FqHWzsgVKcSjqMNEoFb18xdvITGBnKnRmyqg7i%2FQE58%2FfWD4%2FvUrwxeg%2FSCPfP3xm%2BHh8%2FcMT2%2BdPfX60fljQCPeQz3yGxYGAAHEgjQY8o3%2F59Mlj18x6cmI8Wr9%2FfwFkYmBruUFxgxoEALc3EAalv7PiBqqt%2B4ACxQWbpQMA%2BrDn790n%2BHjl%2B8MPDycKN0B5Nj58%2FsXMCY%2BAvPGV6BHPgMLna%2FAovYPw%2F2X3xh2LZn76OfHh%2FuASt9CPfINubsLEEDIaQDUbuHrnLIo5R2DQKWkAAc%2FsDXJwMXFw8DJwwfuu%2FPwCTKwsLMB2%2F3MGLXz%2FPnrGc5ffsJw5f4fBjl5BQY1NXUG8FgYsOD4CgzdY8eOAptgzxncnLQZggLtGcTEhBBDSsAQA2Xqr0BP%2FPwBzBdfv4A9AxrnevzmO8OhbTu%2BvLhxdPWbh6fWgwYxgRhU%2FH6CdXVBNT1AADGiDQWBRigEa3oXlf3mEMoS42djZQF2CjhAAxBAD7FzcgHZPGA%2BaJyXETxoDelEufg0MAD7CQzyimrg%2Bgg0aghu%2BQJNZQf2%2FTU0NBi0tKQYujumMLTU%2BDHoasuBh01Bjv358xvDD1BeAGVsoAdAngF1K56%2B%2F8Fw%2FuTZP3eOb9v8%2Bt7h5UADH4L6a9AY%2BQFvYAA9AhBAjFhGVUCdCZGStjllf3jEkqQFOdhBvUY2Dk4GdiBm4%2BAChjIH0FOcDKxANmLgGtLs%2Bc%2FECGluMGD2JmGWgjwJ9gCwvgI5%2FvcvYD3xE1h3gSpiYAUIkn%2Fw%2BjvDpROnft0%2Ft2fn67uHloE6okD8FIjfQMeE%2FyK37wACiBHbAB20rS%2BSX9GR80NAJVpciEuMi4OVATTiCPIEKwcHuE8PGhwAjQczAT0D8iRoLArkMQZQlxnUSmVigvZJoK1ZYPH%2B8xeweP%2FyGZyU%2FgLzBIj%2FG1jU%2Fv4NqvCAGR2YsZ%2B%2B%2BcFwdN3K99%2Fe3d%2F9%2Bv7RddCkBPMExtApyCMAAcSIa8gU2gMTSs0uD%2FklrBHJJyhkKMHPDh7VADmeGTSIBqJBDmdmA2Zo0LAqCwMrM3SkHsjn5OJlYOFgB%2BcpUCh%2FA2bgH8D0D0o6oBiAeeYvsHkEahg%2Befud4cXL9wxntqy4%2Befb871vH50DZe7nUPwOWndgjP%2BCPAIQQHgHsaExI%2BDs5qsvKqMVziKn78bHyy0qDvIQI6QH%2BA%2Bo7D9owIKZFTxowczMBG5wsgFjjZ2dE1hQ8IPz1i9gyH8F1g0%2FQKXRty8Mv0BJCFgi%2FQU2DZ69%2F8nw%2FsM3hpObVr37%2B%2F3tyW%2Fvbu%2F58u7JDaANr6CY4CA2QAARnFaA9sRAHRkhd98IO2YOUQ92aW1LNm5eMSEBLgYhXg6gZxjhOhiB7X9QjLGxczBw8UBKPB4eQWBd9B1SGn0FlkzAGvvl%2By8M7z7%2FYvj44TPDhR3r3%2F7%2F9eniry9Pj717du08tIh9DY0FoqYVAAKIqIkeaG%2BMEzowJmhq5aLPKyRr%2B%2BMfuyGHsJyGmLQMFzcXMH8AY4iDix1YZLMzgIZcQTEBKrJ5BEXBSegbsJJ99PQFw81bDxlunzrG8O3dk6v%2Ffn269unVzSNfP754BK2x30Fj4BMpEz0AAUTMZOg%2FpCFKEP319LE9IIvOgTyma2Sj8eOphI66tmHBFz4FhoeXzzCwCkozaBtoAxuff8FNb9C0GyhvgUq3rx8%2BMtw6foDhyaWNNT%2B%2BvHkILUo%2FQD3xGVpjfydl6g0EAAKI2Fnd%2F0izrr%2Bh6RVkMeflc0degsaYju1bA6qseIXF5CTFVB3L%2BYUEVVS0VMFDOKC2Eht4dJ2Z4Q%2Bw1n91e38n0BN3oKXRK6gHvkMD6hfStDXRACCASJ2ehlkAix2YhzigmPPtq0ePvn3dmMovJjVVWVNVC%2BQRUPEKKq7%2FA0svUDOFjZX1OzQPPEOrF8iengYIICZy5%2BihFsM88wHqIFAx%2Bfj71w%2B3L%2BycGnfi3PWLoI4RuO0EzOCgwT9QNxpYuv2GJqHP0CKVpGSEDQAEELkeweWpr9BM%2BubHt0%2BPDs8uTjx%2B9OS5r6DSCtgAhI1NQYeu%2FlDqeGQAEEDU8Ai25AcK4U%2FAmHl8dlVzyqlz1858B7alQE2R%2Fwy0AQABxEQjc%2F9DPQPs1rx7fGxOQcqxE%2BdOfAP2a%2F4zMTGQmpGJAQABRCuPoHjm25ePT0%2FNL04%2Fu3%2F38b%2BvnwBrfA7kbipVIgkggBgZaA8YoX0dUOtABFqp%2FocWDm%2Bh%2BYqiGAJViAABRA%2BPIHuGC1pMM0KL3G%2FI3VVKPAIQQPTyCHLbjRlthIji%2FALyCEAAMf7%2F%2F59hOACAAAMAds5QkwALYvYAAAAASUVORK5CYII%3D";
	params.closeSrc = params.closeSrc || "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACoAAAAqCAYAAADFw8lbAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAB2JJREFUeNqsWVtMFFcYPjOz6y5XERQRlYtFFqsIhVKj1CoosErBNq2Vi6aYNk1afWibKBAUmzZINeKLD1VJ0CYNafFCqg9crLVewCYYKVQtiGBRGCitVblfdnd6ZjK7PXP2nJmBdpIvM3N25pxv%2F%2F8%2F%2F20YoP9gZvCcoON54f9anNEY0%2FsHBA1ywkyJMhrkGGyc0SCodVYlzOggSSKEAyePLipg1wJhXFPizDRIimAxgqwKWRpJB%2BValawe9eLEWMq1mkQdyJl0TZK0giyjkySLgSOMaRFFYcfuBQJpBVk1NaOS4jCCHAYGOeNEcXI25Bo908gKQGXnshSSBsoZlyzAFrYjsCFknfeaZBmVTYNK0KABToWog0BwCrm3YWQdpE3GqKicQ6TmhBFHSGioZ35BYfz69etWBQcFhRqNRi8GHpOTk8MDf%2F7F%2F3T1auPRsiO329vahmSCOGzYn0ClK6Cqx4niqkYJznLC29vb%2FE1l5WZraupWk8nk5zTK%2Fv5%2BMD4%2BDsLCwlyGOjU1NVxTV%2F%2Ftzrx3a%2F5%2B8mQUDk0iwAmTNpqLIErMBOEJ4QPhDxEIsQhiCcQyiNgNG1PS%2Bvr7fxQEgZ%2BYmOBPnjzJp6en815eXgokJibyR48e5UdHR3nxWb6v71ryxhSrOIc81wsQiyHmy2v5yGubMJNicKKitMwQ3hCilOZBBEOEQ0RBxKRt2myFC7eKC9fW1vIWi8WNII6YmBi%2BqalJIgulfS9jyxuvy2Sj5LmD5bX85LXNMhcXUZpdOkk71S3%2BQ%2FOixYt9b926VTo%2FMDDi9OnTYPfu3WA6R1FRESgsLASDg4OP4l9O2Pug4%2F5TODwBMU4wAztiBgKH%2BUuSbTrt0lT9%2FYXc6OXL19bV1YG8vLxp52vXrl8HAf7%2BAJrE7Li4OMOpiooWzC05CJFMmprVGY0M65KS5q1b%2B2rGyMiIJEl0S65cuRIkJCS4EZs3dy7YnpsLPDw8XGP5BQWgt7cXrFm9OiPNag1EbJEjRDgXP1bFPaG%2BlPv4k083chxnFlUON4Vr4WVRUaCxsRFcuXIFFOTnu8ZDQ0JAQ0MDOHHiBCg9eNA1brPZQElJCWBZltuzNz9dJWgohMgSkhKW5KriYmPjxQeqqqoULwwPD4OxsTHpev%2F%2B%2FRLZJeHh4NKlS2DhwoXSOLRJxTsXLl4EdrsdJMTHJRCCBU5SQQzdSKhdmmV4wonPQFKzFgQHu6k4NSUFVFZWulQsEvP19ZWuq6urwfYdO9wM%2BObNRhAdHe2Y4%2B%2B%2F9fmzZ2IwGEM21STuV1k9uWikxeINVTVLdOakox5KLycnB0CfKt0rSG4XSbpnct3d3dDnMGx8fLw%2FJQlXHKyeesk%2FIMAkRxjqg10PH4KhoSHF2N27d6m%2BQFS9eJjMZk6PazNoFGPSKl0POiUGQUFBxAcjIiJAXW0tmAt3uZME3Hhg3759wOFwgNLSUreJnXPBHGBYLWHGJapWxwgDA39MPH36bCAgIAAEL1igmMDHx0ci6Vz4%2FPnz4M233nZtsOLiYvDe%2Bx8AG%2FSITtghLNBbwMRlsKuzc1CjlqKqXiDlkr%2B0tDSLP2ZkZCgejoyMdJEUbTIX%2Bs2amlqwdVuWi%2BzatYmKd5KTk8AcPz%2FwQ0NTq0pJotAqR3BLLCGbN3BGw%2BSWzMwNMG6D8vJyyR%2BKU%2FTxfdCB90juSFQ1jOfAAcc7OztBbV295NwPHykDw4j9Hj9%2BAoSHhYIDJYcq7jQ3PUbCp1voxNM8ViV8Ol2Ux%2B3m5s9eio2NKysrA8UHDshTKM1JtEmbg274OTnZ4OtTFaC1reNezLLIAsQtTdBckwhOo1ZSSPnR457e7OysZBiruYddXeDOnV%2FdbQZKVKAE%2FVWrXgHnzp4R3dLkO7k7Dz3quj%2BAJSN2TKIOVPW0utyN7IOOjudDI6P9aampiZmZmZL6xTCp2H0UollZ28CZqu%2BA2WQChZ9%2Feayy4qtblMTZRilHFFmTUU7pvCBmQwSIngQiRPRCECsg4vbk5%2B%2BFLuh3Mb%2F8ra2Nh86eh76W9%2FDw5E0mM2%2BY9S9SrZv4azcapFwUvvN43xeHi8Q55LmWynMHyWvNltc2yVzc8lFaOMXt1eQMr6lW67KyI2Ufrlj%2Bopj5S%2BVHe3s7gPkq3EyMGB6BxbJU2t3i0XLvfvfewqKT9RfOtiC2OEGon4iqZzQqUFrd5DwbPtq167WsrOykxDWro8Uwq7BZQbBdvv5za9XZczfKj5VdRgip1UvESpRULrMq5bKRcJb%2BkKenpwd04qGBgYEmGzCyPb29Y91dHT3jI0MjSLk8RTnbVDoomp06GlmOcM0SNqaABAy0trdPo6anNiC0yOKtHBJJhhCWSWTthNYOtf%2Bkpw9Ka5BpZeWkto6DQs6h0oIU1NqOtL4oSwGj0c0jdfRonTxic1erP0or%2BhhC6aDWGndQeqVaWZOgt%2BOs1RJnVKSJL0Zr2urq5ev92KD2oYHR8WWEmr7p%2FGKiuYCaxKfzCUdQ%2BUqiq4cxk49cM51D%2BC8fxf4RYABOlK9mhtLFVgAAAABJRU5ErkJggg%3D%3D";
	
	
	// Let's set up the default template for a popit display ...
	if(message.search(/^#(\w)*$/) === -1) {
		var popTemplate = '' +
		'<table class="_popContent" id=pop' + __count + '>' +
			'<tr>' +
				'<td style="">' +
					'<img class="_popIcon" src="' + params.iconSrc + '" >' +
				'</td>' +
				
				'<td style="" align="center">' +
					'<span class="_popText">' +
						message +
					'</span>' +
				'</td>' +
				
				'<td style="" valign="top" align="right">' +
					'<img style="cursor:pointer;" onClick="$(\'#pop' + __count + '\').remove();" width="27px" height="27px" src="' + params.closeSrc + '" >' +
				'</td>' +
			'</tr>' +
		'</table>';	
	} else { 
		var popTemplate = '' +
		'<table class="_popContent" id=pop' + __count + '>' +
			'<tr>' +
				'<td style="">' +
					'<img class="_popIcon" src="' + params.iconSrc + '" >' +
				'</td>' +
				
				'<td style="" align="center">' +
					'<span class="_popText">' +
						$(message).html() +
					'</span>' +
				'</td>' +
				
				'<td style="" valign="top" align="right">' +
					'<img style="cursor:pointer;" onClick="$(\'#pop' + __count + '\').remove();" width="27px" height="27px" src="' + params.closeSrc + '" >' +
				'</td>' +
			'</tr>' +
		'</table>';	
	}
	
	// Add in (if any) custom class names

	// ... then set up its container depending if we have
	// already been __initalized or not.
	if(!__init) {
		var popIt = document.createElement("div");
		$(popIt).attr("class","_popContainer");
		
		$("body").append(popIt);
		__init = true;
	}
	
	$("._popContainer").append(popTemplate);
	$("#pop" + __count).hide().fadeIn(params.fadeInTime);
	
	// Check if it's a static message or just a simple
	// delayed pop
	if(params.static !== true) {		
		$("#pop" + __count).delay(params.delay).css("position","relative").animate({
			opacity: 0,
			top: -1000
			}, params.fadeOutTime, function() {
				$(this).remove();
			});
	}
	
	// Listen for a click the the close button
	$("#pop" + __count)
	
	// Return the new pop element
	return $("#pop" + __count);
	
};