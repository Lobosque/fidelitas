#!/bin/bash
# Gera certificados auto-assinados para desenvolvimento do Apple Wallet.
# iOS rejeitará passes assinados com esses certificados.
# Quando tiver Apple Developer Account, substitua pass-cert.p12 pelo certificado real.

set -e
cd "$(dirname "$0")"

echo "Gerando CA de desenvolvimento..."
openssl req -new -x509 -days 365 \
  -keyout ca-key.pem -out ca-cert.pem \
  -subj "/CN=Voltei Dev CA/O=Voltei" \
  -passout pass:voltei-dev 2>/dev/null

echo "Gerando certificado do pass..."
openssl req -new -newkey rsa:2048 -nodes \
  -keyout pass-key.pem -out pass-csr.pem \
  -subj "/CN=Pass Type ID: pass.com.voltei.loyalty/O=Voltei" 2>/dev/null

openssl x509 -req -days 365 \
  -in pass-csr.pem -CA ca-cert.pem -CAkey ca-key.pem \
  -CAcreateserial -out pass-cert.pem \
  -passin pass:voltei-dev 2>/dev/null

echo "Empacotando em .p12..."
openssl pkcs12 -export \
  -out pass-cert.p12 \
  -inkey pass-key.pem -in pass-cert.pem -certfile ca-cert.pem \
  -passout pass: 2>/dev/null

# Limpar arquivos intermediários
rm -f pass-csr.pem ca-cert.srl

echo "Certificados gerados:"
echo "  pass-cert.p12  (certificado do pass, sem senha)"
echo "  ca-cert.pem    (CA de desenvolvimento)"
echo "  ca-key.pem     (chave da CA)"
echo "  pass-cert.pem  (certificado do pass em PEM)"
echo "  pass-key.pem   (chave do pass em PEM)"
echo ""
echo "ATENÇÃO: Esses certificados são apenas para desenvolvimento."
echo "iOS rejeitará passes assinados com eles."
