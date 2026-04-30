
---

# 📁 4️⃣ `docs/TESTING.md`

```md
# 🧪 Testing

---

## Send Message

GET /catalog/send-message

---

## Cart Logs

kubectl logs -f <cart-pod>

---

## Order Logs

kubectl logs -f <order-pod>

---

## DLQ Test

Force error:

throw new Exception("fail");

---

## Check DLQ

az servicebus queue show \
--name order-queue \
--resource-group <rg> \
--namespace-name <ns> \
--query countDetails.deadLetterMessageCount