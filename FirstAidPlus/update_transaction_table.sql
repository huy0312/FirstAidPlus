-- Add new columns for MoMo integration
ALTER TABLE "transactions" ADD COLUMN "momo_order_id" text NULL;
ALTER TABLE "transactions" ADD COLUMN "momo_trans_id" text NULL;
ALTER TABLE "transactions" ADD COLUMN "payment_method" text NOT NULL DEFAULT 'VnPay';
